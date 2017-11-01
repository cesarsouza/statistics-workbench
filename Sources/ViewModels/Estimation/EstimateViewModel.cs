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
    using Microsoft.Win32;
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

    /// <summary>
    ///   Distribution estimation (fitting) view model.
    /// </summary>
    /// 
    public class EstimateViewModel : ViewModelBase
    {
        FormatCollection saveFormats = new FormatCollection
        {
            new CsvFileFormat(), new TsvFileFormat(),
            new XmlFileFormat(), new BinFileFormat()
        };

        FormatCollection loadFormats = new FormatCollection
        {
            new XlsxFileFormat(),
            new XlsFileFormat(), new CsvFileFormat(), new TsvFileFormat(),
            new XmlFileFormat(), new BinFileFormat(), new MatFileFormat()
        };

        private string lastSavePath;
        private IFileFormat lastSaveFormat;



        /// <summary>
        ///   Gets a reference for the parent <see cref="MainViewModel"/>.
        /// </summary>
        /// 
        public MainViewModel Owner { get; private set; }

        /// <summary>
        ///   Gets or sets a value indicating whether the sample editor is shown.
        /// </summary>
        /// 
        public bool IsEditorVisible { get; set; }

        /// <summary>
        ///   Gets or sets a value indicating whether the sample's weights are shown in the sample editor.
        /// </summary>
        /// 
        public bool IsWeightColumnVisible { get; set; }

        /// <summary>
        ///   Gets or sets a value indicating whether the current samples have been modified.
        /// </summary>
        /// 
        public bool IsModified { get; set; }


        /// <summary>
        ///   Gets the sample data shown in the data grid.
        /// </summary>
        /// 
        public BindingList<SampleViewModel> Values { get; private set; }

        /// <summary>
        ///   Gets or sets the error message displayed in the estimation window.
        /// </summary>
        /// 
        public string Message { get; set; }

        /// <summary>
        ///   Gets how many samples should be generated in the estimation window.
        /// </summary>
        /// 
        public int NumberOfSamplesToBeGenerated { get; set; }

        /// <summary>
        ///   Gets or sets the current selected cell.
        /// </summary>
        /// 
        public DataGridCellInfo CurrentCell { get; set; }

        /// <summary>
        ///   Gets or sets the distribution analysis' results.
        /// </summary>
        /// 
        public ObservableCollection<GoodnessOfFitViewModel> Analysis { get; private set; }

        /// <summary>
        ///   Gets or sets a value indicating whether the estimation should be
        ///   continuously updated whenever a sample is changed. Disable this
        ///   property for enhanced performance.
        /// </summary>
        /// 
        public bool IsUpdatedOnEdit { get; set; }


        /// <summary>
        ///   Gets the File->New command.
        /// </summary>
        /// 
        public RelayCommand NewCommand { get; private set; }

        /// <summary>
        ///   Gets the File->Open command.
        /// </summary>
        /// 
        public RelayCommand OpenCommand { get; private set; }

        /// <summary>
        ///   Gets the File->Save command.
        /// </summary>
        /// 
        public RelayCommand SaveCommand { get; private set; }

        /// <summary>
        ///   Gets the File->Import data command.
        /// </summary>
        public RelayCommand ImportCommand { get; private set; }

        /// <summary>
        ///   Gets the File->Estimate command.
        /// </summary>
        /// 
        public RelayCommand EstimateCommand { get; private set; }

        /// <summary>
        ///   Gets the File->Generate command.
        /// </summary>
        /// 
        public RelayCommand GenerateCommand { get; private set; }





        /// <summary>
        ///   Initializes a new instance of the <see cref="EstimateViewModel"/> class.
        /// </summary>
        /// 
        public EstimateViewModel(MainViewModel owner)
        {
            this.Owner = owner;

            this.Values = new BindingList<SampleViewModel>();
            this.Values.ListChanged += data_ListChanged;
            this.Values.Add(new SampleViewModel() { Value = 0, Weight = 1 });

            this.Analysis = new ObservableCollection<GoodnessOfFitViewModel>();

            this.NumberOfSamplesToBeGenerated = 100;
            this.IsUpdatedOnEdit = false;

            this.NewCommand = new RelayCommand(New_Execute);
            this.SaveCommand = new RelayCommand(Save_Execute, Save_CanExecute);
            this.OpenCommand = new RelayCommand(Open_Execute);
            this.EstimateCommand = new RelayCommand(Estimate_Execute, Estimate_CanExecute);
            this.GenerateCommand = new RelayCommand(generate_Execute);

            CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste,
                (sender, e) => Paste_Executed(sender), Paste_CanExecute));
        }




        /// <summary>
        ///  Action for the File->New command.
        /// </summary>
        /// 
        public void New_Execute(object parameter)
        {
            Values.Clear();
            lastSaveFormat = null;
            lastSavePath = null;

            Values.Add(new SampleViewModel() { Value = 0, Weight = 1 });
        }

        /// <summary>
        ///  Action for the File->Save command.
        /// </summary>
        /// 
        public void Save_Execute(object parameter)
        {
            string option = (parameter as string);

            if (!String.IsNullOrEmpty(lastSavePath) && option != "SaveAs")
            {
                save();
                return;
            }

            var dlg = new SaveFileDialog()
            {
                FileName = "Sample",
                DefaultExt = ".csv",
                Filter = saveFormats.GetFilterString(false),
                InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\Samples\\")
            };

            if (!String.IsNullOrEmpty(lastSavePath))
                dlg.InitialDirectory = lastSavePath;

            // Show save file dialog box
            var result = dlg.ShowDialog();

            if (result == true)
            {
                // Get the chosen format
                lastSaveFormat = saveFormats[dlg.FilterIndex - 1];
                lastSavePath = dlg.FileName;

                save();
            }
        }

        /// <summary>
        ///  Action for the File->Open command.
        /// </summary>
        /// 
        public void Open_Execute(object sender)
        {
            var dlg = new OpenFileDialog()
            {
                FileName = "Sample",
                DefaultExt = ".csv",
                Filter = loadFormats.GetFilterString(true),
                FilterIndex = loadFormats.IndexOf("*.csv") + 1,
                InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\Samples\\")
            };

            if (!String.IsNullOrEmpty(lastSavePath))
                dlg.InitialDirectory = lastSavePath;

            // Show open file dialog box
            var result = dlg.ShowDialog();

            if (result == true)
            {
                // Get the chosen format
                var fmt = loadFormats[dlg.FilterIndex - 1];

                DataTable table = fmt.Read(new FileStream(dlg.FileName, FileMode.Open));

                double[][] values = table.ToJagged();

                Values.Clear();
                foreach (double[] row in values)
                {
                    var sample = new SampleViewModel();
                    sample.Value = row[0];
                    if (row.Length > 1)
                        sample.Weight = row[1];
                    Values.Add(sample);
                }
            }
        }

        /// <summary>
        ///  Action for the File->Execute command.
        /// </summary>
        ///
        public void Estimate_Execute(object sender)
        {
            Message = String.Empty;


            double[] values = Values.Select(x => x.Value).ToArray();

            double[] weights = null;

            if (IsWeightColumnVisible)
                weights = Values.Select(x => x.Weight).ToArray();

            var distribution = Owner.SelectedDistribution;

            try
            {
                distribution.Estimate(values, weights);
            }
            catch (System.Exception ex)
            {
                Message += ex.Message + " ";

                try
                {
                    distribution.Estimate(values, null);
                }
                catch (System.Exception ex2)
                {
                    if (ex.Message != ex2.Message)
                        Message += ex2.Message + " ";
                }
            }

            var analysis = new DistributionAnalysis();

            var gof = analysis.Learn(values);

            var fit = System.Linq.Enumerable
                .Select<KeyValuePair<string, GoodnessOfFit>, GoodnessOfFit>(gof, x => x.Value).ToArray();

            this.Analysis.Clear();
            foreach (GoodnessOfFit c in fit.OrderBy(x => x.ChiSquareRank))
            {
                try { this.Analysis.Add(new GoodnessOfFitViewModel(c)); }
                catch { }
            }
        }

        /// <summary>
        ///  Action for the File->Paste command.
        /// </summary>
        ///
        public void Paste_Executed(object target)
        {
            var text = Clipboard.GetText();

            var reader = new CsvReader(new StringReader(text), false, '\t');
            var lines = reader.ReadToEnd();

            lines.Reverse();

            int startIndex = Values.IndexOf(CurrentCell.Item as SampleViewModel);
            if (startIndex == -1)
                startIndex = Values.Count;

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

                Values.Insert(startIndex, sample);
            }
        }



        /// <summary>
        ///   Whether the File->Save command can be executed.
        /// </summary>
        /// 
        public bool Save_CanExecute(object sender)
        {
            return true;
        }

        /// <summary>
        ///   Whether the File->Paste command can be executed.
        /// </summary>
        /// 
        public void Paste_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }


        /// <summary>
        ///   Whether the File->Estimate command can be executed.
        /// </summary>
        /// 
        public bool Estimate_CanExecute(object sender)
        {
            if (Values.Count == 0)
                return false;

            var dist = Owner.SelectedDistribution;

            if (!dist.IsInitialized)
                return false;

            return dist.IsFittable;
        }



        private void generate_Execute(object obj)
        {
            IsEditorVisible = true;
            Message = String.Empty;

            var distribution = Owner.SelectedDistribution;
            var generate = distribution.Instance as ISampleableDistribution<double>;

            try
            {
                double[] values = generate.Generate(samples: NumberOfSamplesToBeGenerated);

                Values.Clear();
                foreach (var d in values)
                    Values.Add(new SampleViewModel() { Value = d });
            }
            catch
            {
                Message = "Sample generation failed. Please check the chosen distribution parameters.";
                return;
            }

            EstimateCommand.Execute(null);

            if (Message != String.Empty)
                Message = "Samples have been generated, but the distribution's measures could not be updated: " + Message;
        }

        private void data_ListChanged(object sender, ListChangedEventArgs e)
        {
            IsModified = true;

            if (IsUpdatedOnEdit && EstimateCommand.CanExecute(this))
                EstimateCommand.Execute(this);
        }

        private void save()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Value", typeof(double));
            table.Columns.Add("Weight", typeof(double));
            foreach (var row in Values)
                table.Rows.Add(row.Value, row.Weight);

            lastSaveFormat.Write(table, new FileStream(lastSavePath, FileMode.Create));

            IsModified = false;
        }
    }
}
