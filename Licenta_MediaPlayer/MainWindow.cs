﻿using System;
using System.Windows.Forms;
using System.Reflection;
using System.IO;

namespace Licenta_MediaPlayer
{
    public partial class MainWindow : Form
    {
        bool mouseOnVolumeTrackbar;
        int soundVolume = 100;
        bool muted = false;
        bool paused = false;

        public MainWindow()
        {
            InitializeComponent();
            //if (this.myVlcControl != null) { this.myVlcControl.Dispose(); this.myVlcControl = null; }
            //this.myVlcControl = new Vlc.DotNet.Forms.VlcControl();
            myVlcControl.VlcLibDirectoryNeeded += OnVlcControlNeedsLibDirectory;
            myVlcControl.EndInit(); //endinit cere ca folderul sa fie setat pt vlccontrol prealabil
        }

        private void OnVlcControlNeedsLibDirectory(object sender, Vlc.DotNet.Forms.VlcLibDirectoryNeededEventArgs e)
        {
            var currentAssembly = Assembly.GetEntryAssembly();
            var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
            if (currentDirectory == null)
                return;

            // de revenit asupra "problemei" 64 biti
            /*if (AssemblyName.GetAssemblyName(currentAssembly.Location).ProcessorArchitecture == ProcessorArchitecture.X86)
            {*/
            e.VlcLibDirectory = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"lib\x86\")); //new DirectoryInfo(Path.Combine(currentDirectory, @"lib\x86\"));
            /*}
            else
            { e.VlcLibDirectory = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"lib\x64\"));//new DirectoryInfo(Path.Combine(currentDirectory, @"lib\x64\"));
            }*/
        }/**/

        private void myVlcControl_Click(object sender, EventArgs e)
        {

        }

        private void button_play_Click(object sender, EventArgs e)
        {
            if (paused)
            {
                myVlcControl.Pause();
                paused = false;
                button_play.Text = "Pause";
            }
            else
            {
                myVlcControl.Pause();
                paused = true;
                button_play.Text = "Play";
            }
        }

        private void button_volume_MouseEnter(object sender, EventArgs e)
        {
            trackBarVolume.Show();
            mouseOnVolumeTrackbar = true;
        }

        private void trackBarVolume_MouseLeave(object sender, EventArgs e)
        {
            trackBarVolume.Hide();
            mouseOnVolumeTrackbar = false;
        }

        private void button_volume_MouseLeave(object sender, EventArgs e)
        {
            //if(mouseOnVolumeTrackbar)
            //  trackBarVolume.Hide();
        }

        private void button_volume_Click(object sender, EventArgs e)
        {
            if (!muted)
            {
                soundVolume = trackBarVolume.Value;
                trackBarVolume.Value = 0;
                muted = true;
            }
            else
            {
                trackBarVolume.Value = soundVolume;
                muted = false;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog oDialog = new OpenFileDialog();
            //ofDialog.Filter = ; 
            //ofDialog.InitialDirectory
            if (oDialog.ShowDialog() == DialogResult.OK)
            {
                playMedia(oDialog.FileName);
            }
        }

        private void playMedia(string filePath)
        {
            myVlcControl.Play(new FileInfo(filePath));
            Text = filePath;
            paused = false;
            button_play.Text = "Pause";
        }

        private void button_stop_Click(object sender, EventArgs e)
        {
            myVlcControl.Stop();
        }

        private void OnVlcMediaLengthChanged(object sender, Vlc.DotNet.Core.VlcMediaPlayerLengthChangedEventArgs e)
        {
            label_toElapse.InvokeIfRequired(l => l.Text = new DateTime(new TimeSpan((long)e.NewLength).Ticks).ToString("T"));
        }

        private void OnVlcPositionChanged(object sender, Vlc.DotNet.Core.VlcMediaPlayerPositionChangedEventArgs e)
        {
            var position = myVlcControl.GetCurrentMedia().Duration.Ticks * e.NewPosition;

            label_elapsed.InvokeIfRequired(l => l.Text = new DateTime((long)position).ToString("T"));
        }

        private void OnVlcPaused(object sender, Vlc.DotNet.Core.VlcMediaPlayerPausedEventArgs e)
        {

        }

        private void OnVlcStopped(object sender, Vlc.DotNet.Core.VlcMediaPlayerStoppedEventArgs e)
        {

        }

        private void OnVlcPlaying(object sender, Vlc.DotNet.Core.VlcMediaPlayerPlayingEventArgs e)
        {
        }
    }

}
