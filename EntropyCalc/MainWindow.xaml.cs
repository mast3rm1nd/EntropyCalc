using System;
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

using System.IO;

namespace EntropyCalc
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if(files.Count() != 1)
                {
                    MessageBox.Show("Поддерживается обработка только одного файла.");
                    return;
                }
                else
                {
                    //var file_Info = new FileInfo(files[0]);
                    var file_Size = new FileInfo(files[0]).Length;
                    var entropy = GetFileEntropy(files[0]);
                    var entropy_UpRounded = Math.Ceiling(entropy);
                    var size_With_Best_Encoding = file_Size * entropy_UpRounded / 8;//Bytes
                    var can_Be_Compressed_Times = file_Size / size_With_Best_Encoding;
                    //var can_Be_Compressed_Percents = file_Size 

                    Label_Text.FontSize = 14;
                    Label_Text.Content = "";
                    Label_Text.Content += String.Format("Файл: {0}", System.IO.Path.GetFileName(files[0])) + "\n";
                    Label_Text.Content += String.Format("Размер: {0} Байт", file_Size) + "\n";
                    Label_Text.Content += String.Format("Информационная энтропия: {0:F2}", entropy) + "\n";
                    Label_Text.Content += String.Format("Размер при эффективнейшем кодировании: {0:F2} Байт", size_With_Best_Encoding) + "\n";
                    if(can_Be_Compressed_Times > 1.00)
                        Label_Text.Content += String.Format("Может быть сжат в {0:F2} раз", can_Be_Compressed_Times) + "\n";
                    else
                        Label_Text.Content += String.Format("Файл не может быть сжат.");
                }
                    
                // Assuming you have one file that you care about, pass it off to whatever
                // handling code you have defined.
                //HandleFileOpen(files[0]);
            }
        }


        double GetFileEntropy(string file_Path)
        {
            var readen_Bytes = File.ReadAllBytes(file_Path);

            uint[] bytes_Statistics = new uint[256];
            var total_Count = 0;

            foreach(Byte Byte in readen_Bytes)
            {
                bytes_Statistics[(int)Byte]++;
                total_Count++;
            }


            var entropy = 0d;
            for(int Byte = 0; Byte <= 255; Byte++)
            {
                if (bytes_Statistics[Byte] == 0)
                    continue;

                var byte_Probability = bytes_Statistics[Byte] / (double)total_Count;
                entropy += byte_Probability * Math.Log(byte_Probability, 2) * -1;
            }

            return entropy;
        }
    }
}
