using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Un4seen.Bass;
using System.Timers;
using Un4seen.Bass.AddOn.Tags;
using System.Drawing;
using System.Threading;

namespace Player
{

    class Engine
    {
        private int stream;
        WeakReference referenceFromEngien;
        public Engine()
        {
            BassInit();
        }

        public Engine(WeakReference reference)
        {
            this.referenceFromEngien = reference;
            this.BassInit();
        }

        ~Engine()
        {
            BassDialoc();
        }

        private void BassInit()
        {
            if (Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero))
            {
                Console.WriteLine("Init bass success");
                BassPlay("");
            }
        }

        public void BassPlay(string thatFile)
        {
            Stop();
            stream = Bass.BASS_StreamCreateFile(thatFile, 0, 0, BASSFlag.BASS_DEFAULT);
            if (stream != 0)
            {
                Bass.BASS_ChannelPlay(stream, false);
                BassSliderMax(); // max value of channel for trackbar
                SetTimer();
                
            }
            else if (stream == 0)
            {
                Console.WriteLine("Error! Format File or you have not chosen the channel");
            }
           
        }

       
        public void BassPause()
        {
            if (Bass.BASS_ChannelIsActive(stream) == BASSActive.BASS_ACTIVE_PLAYING)
            {
                Bass.BASS_ChannelPause(stream);
            }
            else  if ((Bass.BASS_ChannelIsActive(stream) == BASSActive.BASS_ACTIVE_PAUSED))
            {
                Bass.BASS_ChannelPlay(stream,false);
            }
        }

        public void BassSliderMax()  // максимальная продолжительность потока 
        {
            Controller controller = referenceFromEngien.Target as Controller;
            controller.GetLongOfTrack(Convert.ToInt32(Bass.BASS_ChannelBytes2Seconds(stream, Bass.BASS_ChannelGetLength(stream))));
        }

        public void BassTimeLabel(Object sender) //Current time in label timer
        {
            //Current time
            Controller controller = referenceFromEngien.Target as Controller;
            controller.TravelTimerLabel(Convert.ToInt32(Bass.BASS_ChannelBytes2Seconds(stream, Bass.BASS_ChannelGetPosition(stream))));
        }

        public void BassTimeLeft(Object sender) //Left time in second label timer
        {
            Controller controller = referenceFromEngien.Target as Controller;
            int fullTime = Convert.ToInt32(Bass.BASS_ChannelBytes2Seconds(stream, Bass.BASS_ChannelGetLength(stream)));
            int curTime = Convert.ToInt32(Bass.BASS_ChannelBytes2Seconds(stream, Bass.BASS_ChannelGetPosition(stream)));
            controller.TravelLeftTimerLabel(fullTime - curTime);
        }

        public void BassSliderTimeCode(long numberPosition)
        {
            Bass.BASS_ChannelSetPosition(stream, Bass.BASS_ChannelSeconds2Bytes(stream, numberPosition));
        }

        public void BassSliderCurrentTime(Object sourse) //перемещение trackbar || инициализация через timer
        {
            Controller controller = referenceFromEngien.Target as Controller;
            controller.TravelSliderCurrentTime(Convert.ToInt32(Bass.BASS_ChannelBytes2Seconds(stream, Bass.BASS_ChannelGetPosition(stream))));
        }

        public string GetTagsChanel(string path)  //Track name
        {
            TAG_INFO tAG = new TAG_INFO(path);
            return tAG.artist + " - " + tAG.title;
        }

        public Image GetPicture(string path, int i)
        {
            TAG_INFO tAG_INFO = BassTags.BASS_TAG_GetFromFile(path);
            return tAG_INFO.PictureGetImage(i);
        }
        public void BassLevelVolume(int valueOfVolume)
        {
            Bass.BASS_ChannelSetAttribute(stream, BASSAttribute.BASS_ATTRIB_VOL, (float)valueOfVolume / 100);
        }

        private void SetTimer() // this timer inicilize? in BAssPlay
        {
            TimerCallback timerCallback = new TimerCallback(BassTimeLabel);
            System.Threading.Timer currentTimer = new System.Threading.Timer(timerCallback, 0, 0, 1000); //current time in right label

            TimerCallback callbackSlider = new TimerCallback(BassSliderCurrentTime);
            System.Threading.Timer timerSlider = new System.Threading.Timer(callbackSlider, 0, 0, 1000); // current position of slider 

            TimerCallback callbackLeftTime = new TimerCallback(BassTimeLeft);
            System.Threading.Timer timerLeft = new System.Threading.Timer(callbackLeftTime, 0, 0, 1000); // count down timer (left label timer)
        }

        private void Stop()
        {
            Bass.BASS_ChannelStop(stream);
        }
       

        private void BassDialoc()
        {
            // free the stream
            Bass.BASS_StreamFree(stream);
            // free BASS
            Bass.BASS_Free();
            
        }
    }
}
