using Amazon.S3.Model;
using System;
using System.IO;
using System.IO.Ports;
using System.Web;
using System.Windows.Forms;
using XModemProtocol;
using System.Diagnostics;
using System.Threading;


namespace myTerminal6
{

    public partial class Form1 : Form
    {

        string gelenveri = string.Empty;
        string[] parcalar;

        public Form1()
        {
            InitializeComponent();
            
            string[] ports = SerialPort.GetPortNames();

            foreach (string port in ports)
            {
                comboBox1.Items.Add(port);
            }
        }

        public void button1_Click(object sender, EventArgs e)
        {

            if (serialPort1.IsOpen)
            {
                string uzanti;

                //XModemCommunicator başlat.
                var xmodem = new XModemCommunicator();


                // Port ekle
                xmodem.Port = serialPort1;

                // Datayı al.
                OpenFileDialog dialog = new OpenFileDialog();

                dialog.Filter = "Binary Files (*.bin) |*.bin|Hex Files (*.hex) |*.hex"; ;

                openFileDialog.InitialDirectory = @"C:\";
                
                
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                      uzanti = Path.GetExtension(dialog.FileName);

                    if (uzanti.Equals(".hex", StringComparison.OrdinalIgnoreCase))
                    {
                        string filepath = string.Empty;
                        string outputFilepath = string.Empty;
                        filepath = " "+dialog.FileName+" ";
                        outputFilepath = Environment.GetFolderPath(
                        Environment.SpecialFolder.Desktop);
                        outputFilepath += "\\bin.bin";
                        Process p = new Process();
                        string projedizini = Environment.CurrentDirectory;
                        p.StartInfo.FileName =projedizini + "\\MinGW\\bin\\objcopy.exe";
                        p.StartInfo.Arguments = "--input-target=ihex --output-target=binary" + filepath + outputFilepath;
                        p.Start();
                        dialog.FileName = outputFilepath;
                        Thread.Sleep(1000);
                    }
                    xmodem.Data = File.ReadAllBytes(dialog.FileName);
                }
                
                // Datayı gönder.
                xmodem.Mode = XModemMode.CRC;
                xmodem.Mode.Equals(xmodem.Mode);
                xmodem.Send();

                gelenveri = string.Empty;
                textGelenveri.Text = " ";
                txtVersion.Text = " ";
                // Datanın gönderildiğini kontrol et.
                if (xmodem.State != XModemStates.Idle)
                {
                    xmodem.CancelOperation();

                }
                string dosyaAdi = Path.GetFileName(dialog.FileName);
                if (dosyaAdi.Equals("bin.bin", StringComparison.OrdinalIgnoreCase))
                {
                    File.Delete(dialog.FileName);
                }
                    
                
                
            }

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnAc_Click(object sender, EventArgs e)
        {
            // Seri portu aç.
            if (!serialPort1.IsOpen && comboBox1.Text != string.Empty)
            {
                serialPort1.BaudRate = Convert.ToInt32(textBound.Text);
                
                serialPort1.PortName = comboBox1.Text;
                serialPort1.Open();
            }
            
        }
        
        private void btnKapa_Click(object sender, EventArgs e)
        {
            // Seri portu kapat.
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
            }

        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // C karakteri haricinde mikrodenetleyiciden gelen verileri textbox'a yazdır.
            if (!gelenveri.Contains("C"))
            {
                gelenveri = serialPort1.ReadExisting();
                parcalar = gelenveri.Split('&');
                this.Invoke(new EventHandler(DisplayText));

                
               
            }
           

        }

        private void DisplayText(object sender, EventArgs e)
        {
            // Event handler
             textGelenveri.Text = textGelenveri.Text + parcalar[0];
            if (1 < parcalar.Length)
            {
                txtVersion.Text = parcalar[1];
            }
        }

        private void btnBoot_Click(object sender, EventArgs e)
        {
            // Bootloader moduna girmek için mikrodenetleyiciye anahtar gönder.
            if(serialPort1.IsOpen)
            {
                serialPort1.Write("1");
                textGelenveri.Text = " ";
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
