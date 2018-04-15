using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tekla.Structures.Model;
using T3D = Tekla.Structures.Geometry3d;
using TSMUI = Tekla.Structures.Model.UI;

namespace BeamsToText
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Model Model;

        public MainWindow()
        {
            InitializeComponent();

            try { Model = new Model(); }
            catch { throw new Exception("Tekla is not connected"); }
        }

        private void Button_SaveSelected_Click(object sender, RoutedEventArgs e)
        {
            WriteDataFile(false);

        }

        private void Button_SaveAll_Click(object sender, RoutedEventArgs e)
        {
            WriteDataFile(true);
        }

        private void WriteDataFile(bool AllObjects)
        {
            ModelObjectEnumerator ModelObjectsToWriteOut = null;
            if(AllObjects)
            {
                ModelObjectsToWriteOut = Model.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.BEAM);
            }
            else
            {
                TSMUI.ModelObjectSelector GetSelectedObjects = new TSMUI.ModelObjectSelector();
                ModelObjectsToWriteOut = GetSelectedObjects.GetSelectedObjects();
            }

            string FileName = "BeamToTextFile.txt";
            string FinalFileName = System.IO.Path.Combine(Model.GetInfo().ModelPath, FileName);
            using (StreamWriter FileWriter = new StreamWriter(FinalFileName))
            {
                while (ModelObjectsToWriteOut.MoveNext())
                {
                    Beam ThisBeam = ModelObjectsToWriteOut.Current as Beam;
                    if (ThisBeam != null)
                    {
                        string DataLineForFile = Model.GetGUIDByIdentifier(ThisBeam.Identifier) + ", "
                            + ThisBeam.Profile.ProfileString + ", "
                            + ThisBeam.Material.MaterialString + ", "
                            + ThisBeam.Class;
                        FileWriter.WriteLine(DataLineForFile);
                    }
                }
            }

            MessageBox.Show("File Exported");
            Tekla.Structures.Model.Operations.Operation.DisplayPrompt("File Exported and written to Model Folder");
        }
    }
}
