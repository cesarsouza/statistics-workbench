@echo off

echo.
echo Statistics Workbench :: Build and Packaging script
echo ===================================================
echo. 
echo This Windows batch file will use Visual Studio 2013 to compile
echo and pack the binaries and sources of the Statistics Workbench.
echo. 

:: Configuration
set version=1.0.0.1

:: Executables
set devenv="C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe"
set rar="C:\Program Files\WinRAR\rar"
set opts=a -m5 -s

:: File names
set binPath="Statistics-Workbench-%version%-bin.rar" 
set srcPath="Statistics-Workbench-%version%-src.rar"

echo.
echo  - Building Release configuration...
%devenv% "Statistics Workbench.sln" /Rebuild Release




echo.
echo Creating Statistics Workbench %binPath% archive
echo ---------------------------------------------------------

timeout /T 5
set output=Binaries\%srcPath%
del %output%

%rar% %opts% -r %output% ".\*" -x*\.git -x*\obj -x*\bin -x*\TestResults -x*.sdf -x*.suo -x*.user -x*.shfbproj_* -x*.vsp -x*.pidb -x*\packages -x*\Binaries -x*.pdb -x*.GhostDoc.xml
%rar% t         %output%


echo.
echo Creating Accord.NET %binPath% archive
echo ---------------------------------------------------------

timeout /T 5
set output=Binaries\%binPath%
del %output%

%rar% %opts%    %output% "Binaries\Release" -x*\.svn* -x*\.git* -x*.lastcodeanalysissucceeded -x*.CodeAnalysisLog.xml -x*.pdb
%rar% %opts%    %output% "LICENSE.txt"
%rar% %opts%    %output% "README.md"
%rar% t         %output%



echo.
echo ---------------------------------------------------------
echo Package creation has completed. Please check the above
echo commands for errors and check packages in output folder.
echo ---------------------------------------------------------
echo.

timeout /T 10
