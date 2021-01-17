using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Player
{
    class Controller
    {
        private Engine audioEngine;
        private PlayerUI mainWindow;

        public Controller()
        {
            this.InitController();
        }

        public void TravelSliderTimeCode(int numberposition)
        {
            audioEngine.BassSliderTimeCode(Convert.ToInt64(numberposition));
        }

        public void TravelSliderCurrentTime(int currentTime)
        {
            mainWindow.SliderCurrentTime(currentTime);
        }

        public void TravelTimerLabel(int currentTimeLab)  // current time to label Timer
        {
            var timespain = TimeSpan.FromSeconds(currentTimeLab);
            mainWindow.TimerLabel(timespain.ToString(@"mm\:ss"));
        }

        public void TravelLeftTimerLabel(int leftTimer)
        {
            var timespain = TimeSpan.FromSeconds(leftTimer);
            mainWindow.TimerLeftLabel(timespain.ToString(@"mm\:ss"));
        }

        public void GetLongOfTrack(int secTreck)  //получение длительности потока 
        {
            mainWindow.SliderMaxValue(secTreck);
        }

        public string TravelTagChanal(string path)  // track name 
        {
          return audioEngine.GetTagsChanel(path);
        }
        public void TravelSliderVolume(int valueOfVolume)
        {
            audioEngine.BassLevelVolume(valueOfVolume);
        }

        public Image TravelImage(string path, int i)
        {
           return audioEngine.GetPicture(path, i);
        }

        public void PlayFile(string file)
        {
            audioEngine.BassPlay(file);
        }

        public void PlayPause()
        {
            audioEngine.BassPause();
        }

        ~Controller()
        {
            audioEngine = null;
            mainWindow = null;
        }

        private void InitController()
        {
            WeakReference referenceFromEngien = new WeakReference(this); // this too
            WeakReference reference = new WeakReference(this);
            audioEngine = new Engine(referenceFromEngien); // last change
            mainWindow = new PlayerUI(reference);
            Application.Run(this.mainWindow);
        }
    }
}
