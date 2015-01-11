// Statistics Workbench
// http://accord-framework.net
//
// The MIT License (MIT)
// Copyright © 2014-2015, César Souza
//

namespace Workbench.ViewModels
{
    using Accord.IO;
    using Accord.Math;
    using Accord.Statistics.Analysis;
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Distributions.Fitting;
    using Microsoft.Win32;
    using PropertyChanged;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Workbench.Formats;
    using Workbench.Framework;
    using Workbench.Tools;

    /// <summary>
    ///   Main view model associated with the main application window.
    /// </summary>
    /// 
    [ImplementPropertyChanged]
    public class MainViewModel : ViewModelBase
    {

        /// <summary>
        ///   Gets all distributions that can be selected.
        /// </summary>
        /// 
        public ObservableCollection<DistributionViewModel> Distributions { get; private set; }

        /// <summary>
        ///   Gets or sets the index of the current selected distribution.
        /// </summary>
        /// 
        public int SelectedDistributionIndex { get; set; }

        /// <summary>
        ///   Gets the current selected distribution.
        /// </summary>
        /// 
        public DistributionViewModel SelectedDistribution
        {
            get { return Distributions[SelectedDistributionIndex]; }
        }



        public bool ShowEditor { get; set; }


        public bool IsContinuous { get; set; }

        public bool HasModifications { get; set; }

        public string LastSavePath { get; set; }

        public IFileFormat LastSaveFormat { get; set; }

        public BindingList<SampleViewModel> Data { get; private set; }

        public string Message { get; set; }
        public int Samples { get; set; }

        public DataGridCellInfo CurrentCell { get; set; }


        public RelayCommand NewCommand { get; private set; }
        public RelayCommand OpenCommand { get; private set; }
        public RelayCommand SaveCommand { get; private set; }

        public RelayCommand ImportCommand { get; private set; }
        public RelayCommand RefreshCommand { get; private set; }
        public RelayCommand GenerateCommand { get; private set; }



        private FormatCollection SaveFormats = new FormatCollection
        {
            new CsvFileFormat(),
            new TsvFileFormat(),
            new XmlFileFormat(),
            new BinFileFormat()
        };

        private FormatCollection LoadFormats = new FormatCollection
        {
            new XlsxFileFormat(),
            new XlsFileFormat(),
            new CsvFileFormat(),
            new TsvFileFormat(),
            new XmlFileFormat(),
            new BinFileFormat(),
            new MatFileFormat()
        };


        public ObservableCollection<GoodnessOfFit> Analysis { get; set; }


        /// <summary>
        ///   Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        /// 
        public MainViewModel()
        {
            // Create ViewModels for each statistical distribution
            var distributions = DistributionManager.GetDistributions();
            this.Distributions = new ObservableCollection<DistributionViewModel>(distributions);
            this.SelectedDistributionIndex = distributions.Find(x => x.Name == "Normal")[0];

            this.NewCommand = new RelayCommand(New_Execute);
            this.SaveCommand = new RelayCommand(Save_Execute, Save_CanExecute);
            this.OpenCommand = new RelayCommand(Open_Execute);
            this.RefreshCommand = new RelayCommand(Refresh_Execute, Refresh_CanExecute);
            this.GenerateCommand = new RelayCommand(Generate_Execute);

            CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, Paste_Executed, Paste_CanExecute));

            this.IsContinuous = false;
            this.Data = new BindingList<SampleViewModel>();
            this.Data.ListChanged += Data_ListChanged;
            this.Data.Add(new SampleViewModel() { Value = 0, Weight = 1 });

            this.Analysis = new ObservableCollection<GoodnessOfFit>();

            this.Samples = 1000;
        }


        void Data_ListChanged(object sender, ListChangedEventArgs e)
        {
            HasModifications = true;

            if (IsContinuous && RefreshCommand.CanExecute(this))
                RefreshCommand.Execute(this);
        }

        public void New_Execute(object parameter)
        {
            Data.Clear();
            LastSaveFormat = null;
            LastSavePath = null;

            Data.Add(new SampleViewModel() { Value = 0, Weight = 1 });
        }


        public void Save_Execute(object parameter)
        {
            string option = (parameter as string);

            if (!String.IsNullOrEmpty(LastSavePath) && option != "SaveAs")
            {
                save();
                return;
            }

            var dlg = new SaveFileDialog()
            {
                FileName = "Sample",
                DefaultExt = ".csv",
                Filter = SaveFormats.GetFilterString(false)
            };

            // Show save file dialog box
            var result = dlg.ShowDialog();

            if (result == true)
            {
                // Get the chosen format
                LastSaveFormat = SaveFormats[dlg.FilterIndex - 1];
                LastSavePath = dlg.FileName;

                save();
            }
        }

        private void save()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Value", typeof(double));
            table.Columns.Add("Weight", typeof(double));
            foreach (var row in Data)
                table.Rows.Add(row.Value, row.Weight);

            LastSaveFormat.Write(table, new FileStream(LastSavePath, FileMode.Create));

            HasModifications = false;
        }

        public bool Save_CanExecute(object sender)
        {
            return true;
        }

        public void Open_Execute(object sender)
        {
            var dlg = new OpenFileDialog()
            {
                FileName = "Sample",
                DefaultExt = ".csv",
                Filter = LoadFormats.GetFilterString(true)
            };

            // Show open file dialog box
            var result = dlg.ShowDialog();

            if (result == true)
            {
                // Get the chosen format
                var fmt = LoadFormats[dlg.FilterIndex - 1];

                DataTable table = fmt.Read(new FileStream(dlg.FileName, FileMode.Open));

                double[][] values = table.ToArray();

                Data.Clear();
                foreach (double[] row in values)
                {
                    var sample = new SampleViewModel();
                    sample.Value = row[0];
                    if (row.Length > 1)
                        sample.Weight = row[1];
                    Data.Add(sample);
                }
            }
        }


        public void Paste_Executed(object target, ExecutedRoutedEventArgs e)
        {
            var text = Clipboard.GetText();

            var reader = new CsvReader(new StringReader(text), false);
            var lines = reader.ReadToEnd();

            lines.Reverse();

            int startIndex = Data.IndexOf(CurrentCell.Item as SampleViewModel);
            if (startIndex == -1)
                startIndex = Data.Count;

            foreach (var line in lines)
            {
                var sample = new SampleViewModel();

                double value;
                if (!Double.TryParse(line[0], out value))
                    continue;

                sample.Value = value;

                if (line.Length > 1)
                {
                    double weight = 1;
                    if (Double.TryParse(line[1], out weight))
                        sample.Weight = weight;
                }

                Data.Insert(startIndex, sample);
            }
        }

        public void Paste_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public void Refresh_Execute(object sender)
        {
            Message = String.Empty;

            double[] values = Data.Select(x => x.Value).ToArray();
            double[] weights = Data.Select(x => x.Weight).ToArray();

            try
            {
                SelectedDistribution.Estimate(values, weights);
            }
            catch (System.Exception ex)
            {
                Message += ex.Message + " ";

                try
                {
                    SelectedDistribution.Estimate(values, null);
                }
                catch (System.Exception ex2)
                {
                    if (ex.Message != ex2.Message)
                        Message += ex2.Message + " ";
                }
            }

            var analysis = new DistributionAnalysis(values);
            analysis.Compute();

            var fit = System.Linq.Enumerable
                .Select<KeyValuePair<string, GoodnessOfFit>, GoodnessOfFit>(
                analysis.GoodnessOfFit,
                x => x.Value).ToArray();

            this.Analysis.Clear();
            foreach (GoodnessOfFit c in fit.OrderBy(x => x.KolmogorovSmirnovRank))
                this.Analysis.Add(c);
        }

        public bool Refresh_CanExecute(object sender)
        {
            return Data.Count > 0;
        }

        private void Generate_Execute(object obj)
        {
            var generate = SelectedDistribution.Instance as ISampleableDistribution<double>;
            double[] values = generate.Generate(Samples);
            Data.Clear();
            foreach (var d in values)
                Data.Add(new SampleViewModel() { Value = d });
            ShowEditor = true;
            RefreshCommand.Execute(null);
        }


    }
}
