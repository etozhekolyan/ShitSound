using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;



namespace Player
{
    class PlayerUI : Form
    {
        private TrackBar trackSlider;
        private Button openFile;
        private Button pauseButton;
        private Button nextButton;
        private Button privioslyButton;
        private Button openTrackList;
        private Label trackName;
        private Label currentTime;
        private Label countdown;
        private PictureBox pictureTrack;
        private WeakReference referencePause;
        private OpenFileDialog fileDialog;
        private TrackBar sliderVolume;
        private ListBox tracklist;

        private List<string> files = new List<string>();

        public PlayerUI()
        {
            this.ConfigureIUElements();
            this.ConfigureWindow();
        }

        public PlayerUI(WeakReference reference)
        {
            this.referencePause = reference;
            this.ConfigureWindow();
            this.ConfigureIUElements();
        }

        ~PlayerUI()
        {
            this.Deinit();
        }

        private void ConfigureWindow()
        {
            this.Size = new Size(500, 500);
            this.CenterToScreen();
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
        }

        private void ConfigureIUElements()
        {
            this.trackSlider = new TrackBar();
            trackSlider.Size = new Size(375, 25);
            trackSlider.Location = new Point(55, 300);
            trackSlider.Minimum = 0;
            trackSlider.Maximum = 10;
            trackSlider.TickFrequency = 1;
            trackSlider.LargeChange = 3;
            trackSlider.Scroll += new EventHandler(TrackSliderMove);
            this.Controls.Add(trackSlider);

            sliderVolume = new TrackBar();
            sliderVolume.Size = new Size(100, 25);
            sliderVolume.Location = new Point(194, 425);
            sliderVolume.Maximum = 100;
            sliderVolume.Minimum = 0;
            sliderVolume.LargeChange = 1;
            sliderVolume.Value = 5;
            sliderVolume.Scroll += new EventHandler(SliderScrollVolume);
            //sliderVolume.TickFrequency = 1;
            this.Controls.Add(sliderVolume);

            openFile = new Button();
            openFile.Size = new Size(50, 30);
            openFile.Text = "OPEN";
            openFile.Location = new Point(10, 10);
            openFile.Click += new EventHandler(OpenFileButton);
            this.Controls.Add(openFile);

            pauseButton = new Button();
            pauseButton.Size = new Size(40, 50);
            pauseButton.Text = "||";
            pauseButton.Click += new EventHandler(PauseButtonMove);
            pauseButton.Location = new Point(220, 350);
            this.Controls.Add(pauseButton);

            nextButton = new Button();
            nextButton.Size = new Size(40, 50);
            nextButton.Location = new Point(270, 350);
            nextButton.Text = ">";
            this.Controls.Add(nextButton);

            privioslyButton = new Button();
            privioslyButton.Size = new Size(40, 50);
            privioslyButton.Text = "<";
            privioslyButton.Click += new EventHandler(PriviosleButtonPressed);
            privioslyButton.Location = new Point(170, 350);
            this.Controls.Add(privioslyButton);

            trackName = new Label();
            trackName.Size = new Size(200, 50);
            trackName.Location = new Point(55, 280);
            trackName.Text = "Unknow";
            this.Controls.Add(trackName);

            currentTime = new Label();
            currentTime.Size = new Size(50, 25);
            currentTime.Location = new Point(50, 355);
            currentTime.Text = "00:00";
            this.Controls.Add(currentTime);

            countdown = new Label();
            countdown.Size = new Size(50, 25);
            countdown.Location = new Point(400, 355);
            countdown.Text = "00:00";
            this.Controls.Add(countdown);

            pictureTrack = new PictureBox();
            pictureTrack.Size = new Size(210, 210);
            pictureTrack.Location = new Point(140, 30);
            pictureTrack.SizeMode = PictureBoxSizeMode.StretchImage;
            //pictureTrack.Image = Image.FromFile("C:/Users/Etozhekolyan/Desktop/project/defoult.jpg"); //необработанное исключене
            this.Controls.Add(pictureTrack);

            openTrackList = new Button();
            openTrackList.Size = new Size(50, 25);
            openTrackList.Location = new Point(425, 10);
            openTrackList.Text = "LIST";
            openTrackList.Click += new EventHandler(OpenListButton);
            this.Controls.Add(openTrackList);

            tracklist = new ListBox();
            tracklist.Size = new Size(300, 446);
            tracklist.Location = new Point(500, 10);
            this.Controls.Add(tracklist);
                   

        }

        private void OpenFileButton(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void OpenListButton(object sender, EventArgs e)
        {
            if (this.Width == 500) this.Width = 825;
            else if (this.Width == 825) this.Width = 500;
        }
        private void TrackSliderMove(object sender, EventArgs e)
        {
            SliderTimeCode();
        }

        private void SliderScrollVolume(object sender, EventArgs e)
        {
            Controller controller = referencePause.Target as Controller;
            controller.TravelSliderVolume(sliderVolume.Value);
        }

        private void PauseButtonMove(object sender, EventArgs e)
        {
            PauseChange();
            Controller controller = referencePause.Target as Controller;
            controller.PlayPause();
        }

        private void OpenFile()
        {
            Controller controller = referencePause.Target as Controller;
            fileDialog = new OpenFileDialog();
            fileDialog.Filter = "MP3|*.mp3";
            fileDialog.Multiselect = true;
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                //controller.PlayFile(fileDialog.FileName);
                //files.Add(fileDialog.FileName);
                foreach (string wayFile in fileDialog.FileNames)
                {
                    files.Add(wayFile);
                    Console.WriteLine("from files " + files);
                }
                //tracklist.Items.Add(GetNameTrack(fileDialog.FileNames));
                GetNameTrack(fileDialog.FileNames);
                controller.PlayFile(files[tracklist.SelectedIndex]);
                controller.TravelSliderVolume(sliderVolume.Value);
                pictureTrack.Image = controller.TravelImage(fileDialog.FileName, 0);
                trackName.Text = controller.TravelTagChanal(fileDialog.FileName); // return name of chanel 
            }
        }
        private void GetNameTrack(string [] file) // получается массив расположений файлов и передает в tracklist только название
        {
            for (int i = 0; i < file.Length; i++)
            {
                string [] names = file[i].Split('\\');
                tracklist.Items.Insert(i, names[names.Length -1]);
            }
        }
        public void TimerLabel(string time)
        {
            currentTime.Text = time;
        }
        public void TimerLeftLabel(string time)
        {
            countdown.Text = time;
        }

        public void SliderMaxValue(int secTreck)
        {
            this.trackSlider.Maximum = secTreck;
            Console.WriteLine("Max value slider fron UI " + Convert.ToUInt32(this.trackSlider.Maximum)); // test max value of slider
        }

        public void SliderTimeCode()
        {
            Controller controller = referencePause.Target as Controller;
            controller.TravelSliderTimeCode(trackSlider.Value);
            Console.WriteLine("Current position slider is " + (int)trackSlider.Value);
        }

        public void SliderCurrentTime(int currentTime)
        {
            trackSlider.Value = currentTime;
        }

        private void PriviosleButtonPressed(object sender, EventArgs e)
        {
            trackSlider.Value = 0;
        }
        
        
        private void PauseChange()
        {
            if (pauseButton.Text == "||")
            {
                pauseButton.Text = ">";
            }
            else if (pauseButton.Text == ">")
            {
                pauseButton.Text = "||";
            }
            

        }

        private void Deinit()
        {
            this.trackSlider = null;
        }
    }
}
