﻿using System;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Globalization;
using System.Threading.Tasks;

namespace Licenta_MediaPlayer
{
    public partial class MainWindow : Form
    {
        bool mouseOnVolumeTrackbar;
        int soundVolume = 100;
        bool muted = false;
        bool paused = false;
        bool userIsPositioningTrackBar = false;
        bool isFullscreen = false;
        string recordFolder = Application.StartupPath + @"\rec";
        string MRL = "";
        string RecordingFileName = "";

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
            trackBarElapsed.Focus();
        }

        private void button_volume_MouseEnter(object sender, EventArgs e)
        {
            /*trackBarVolume.Show();
            mouseOnVolumeTrackbar = true;*/
        }

        private void trackBarVolume_MouseLeave(object sender, EventArgs e)
        {
           /* trackBarVolume.Hide();
            mouseOnVolumeTrackbar = false;*/
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
                MRL = myVlcControl.GetCurrentMedia().Mrl;                
            }
        }

        private void playMedia(string filePath)
        {
            myVlcControl.Play(new FileInfo(filePath));
            Text = filePath;
            paused = false;
            button_play.Text = "Pause";
            trackBarElapsed.InvokeIfRequired(t => t.Maximum = (int)(myVlcControl.GetCurrentMedia().Duration.TotalSeconds));
        }

        private void button_stop_Click(object sender, EventArgs e)
        {
            myVlcControl.Stop();
        }

        private void OnVlcMediaLengthChanged(object sender, Vlc.DotNet.Core.VlcMediaPlayerLengthChangedEventArgs e)
        {
            label_toElapse.InvokeIfRequired(l => l.Text = new DateTime(new TimeSpan((long)e.NewLength).Ticks).ToString("T"));
            //trackBarElapsed.InvokeIfRequired (t => t.Maximum= (int)(myVlcControl.GetCurrentMedia().Duration.TotalMilliseconds));  // schimbarea aici a dimensiunii seekbarului
                                                                                                                                    // esueaza (probabil ca informatiile fisierului
                                                                                                                                    // sunt incarcate dupa acest punct
        }

        private void OnVlcPositionChanged(object sender, Vlc.DotNet.Core.VlcMediaPlayerPositionChangedEventArgs e)
        {
            if (userIsPositioningTrackBar == false)
            {
                var position = myVlcControl.GetCurrentMedia().Duration.Ticks * e.NewPosition;
                label_elapsed.InvokeIfRequired(l => l.Text = new DateTime((long)position).ToString("T"));
                trackBarElapsed.InvokeIfRequired(t => t.Value = (int)(myVlcControl.Time/1000));
                //trackBarElapsed.InvokeIfRequired(t => t.Value = (int)(myVlcControl.Position*(float)myVlcControl.GetCurrentMedia().Duration.TotalMilliseconds));
            }
        }

        private void OnVlcPaused(object sender, Vlc.DotNet.Core.VlcMediaPlayerPausedEventArgs e)
        {

        }

        private void OnVlcStopped(object sender, Vlc.DotNet.Core.VlcMediaPlayerStoppedEventArgs e)
        {

        }

        private void OnVlcPlaying(object sender, Vlc.DotNet.Core.VlcMediaPlayerPlayingEventArgs e)
        {
            trackBarElapsed.InvokeIfRequired(t => t.Maximum = (int)(myVlcControl.GetCurrentMedia().Duration.TotalSeconds)); // de fiecare data cand clipul porneste/iese din pauza
                                                                                                                            // as putea folosi o variabila globala...
        }

        private void trackBarVolume_ValueChanged(object sender, EventArgs e)
        {
            myVlcControl.InvokeIfRequired(v => v.Audio.Volume = trackBarVolume.Value);
        }

        private void trackBarElapsed_MouseDown(object sender, MouseEventArgs e)
        {
            // sari la locatia clickuita
            double dblValue = ((double)e.X / (double)trackBarElapsed.Width) * (trackBarElapsed.Maximum - trackBarElapsed.Minimum);
            trackBarElapsed.Value = Convert.ToInt32(dblValue);
            //trackBarElapsed.InvokeIfRequired(t => t.Value = Convert.ToInt32(dblValue));

            userIsPositioningTrackBar = true;
        }

        private void trackBarElapsed_MouseUp(object sender, MouseEventArgs e)
        {
            myVlcControl.InvokeIfRequired(v => v.Time = 1000*Convert.ToInt32(trackBarElapsed.Value, CultureInfo.CurrentCulture));
            myVlcControl.Play();

            userIsPositioningTrackBar = false;
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }
        
        void RecordMedia()
        {
            vlcStop();
            RecordingFileName = "";
            string finalfilename = "";

            if (this.myVlcControl!= null && !string.IsNullOrEmpty(MRL) && !string.IsNullOrEmpty(recordFolder))
            {
                try
                {

                    if (Directory.Exists(recordFolder))
                    { // video files directory

                        string data = "";
                        try { data = ("-" + label_elapsed.Text + "-" + GetClock()).Replace(':', '-'); } catch { data = ""; }
                        finalfilename = recordFolder + "\\" + "REC" + data + ".mp4";

                        var options = new string[] { @":sout=#duplicate{dst=display,dst=std{access=file,mux=ts,dst='" + finalfilename + @"'}}" }; 
                                                                                    // pt formatele (video) nesuportate de VLC salveaza doar audio ex: .mkv
                                                                                    // pt 3gp nu faci export audio
                                                                                    // wmv de asemenea nu merge (sau nu ia audio si video poate fi vazut doar pe anumite playere?) 
                        // play & record
                        RecordingFileName = finalfilename;
                        try
                        {
                            if (!this.myVlcControl.IsPlaying)
                            {
                                this.myVlcControl.Play(new Uri(MRL), options);
                                //createDelay(1000);
                            };
                        }
                        catch
                        {
                            vlcStop();
                        }
                    }

                }
                catch { }
            }

            //getMediaDuration();
        }

        void vlcStop()
        {
            RecordingFileName = "";
            myVlcControl.Stop();
        }

        private void button_record_Click(object sender, EventArgs e)
        {
            RecordMedia();
        }

        string GetClock()
        {
            string ClockInstring = "";
            // Get current time:
            int hour = DateTime.Now.Hour;
            int min = DateTime.Now.Minute;
            int sec = DateTime.Now.Second;
            // Format current time into string:
            ClockInstring = (hour < 10) ? "0" + hour.ToString() : hour.ToString();
            ClockInstring += ":" + ((min < 10) ? "0" + min.ToString() : min.ToString());
            ClockInstring += ":" + ((sec < 10) ? "0" + sec.ToString() : sec.ToString());
            return ClockInstring;
        }

        private void shareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*var facebookClient = new FacebookClient();
            var facebookService = new FacebookService(facebookClient);
            var getAccountTask = facebookService.GetAccountAsync(FacebookSettings.AccessToken);
            Task.WaitAll(getAccountTask);
            var account = getAccountTask.Result;
            Console.WriteLine($"{account.Id} {account.Name}");

            var postOnWallTask = facebookService.PostOnWallAsync(FacebookSettings.AccessToken,
            "Hello from C# .NET Core!");
            Task.WaitAll(postOnWallTask);*/

            Form1 fbForm = new Form1();
            fbForm.Show();
            this.Hide();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            panel1.Dock = DockStyle.Fill; // deoarece MouseEvent-urile sunt dezactivate in timp ce Vlccontrol reda media
                                          // folosesc un panou ce acopera tot vlccontrolul si are ca mouse event fullscreen pe 2xclick
            //panel1.BackColor = System.Drawing.Color.Transparent;
            myVlcControl.Controls.Add(panel1);
        }

        private void myVlcControl_Click_1(object sender, EventArgs e)
        {
            
        }

        private void fullscreen()
        {        
            //MessageBox.Show("Fullscr");
            if (isFullscreen)
            {
                this.MaximizeBox = true;
                this.WindowState = FormWindowState.Normal;
                this.FormBorderStyle = FormBorderStyle.Sizable;
                isFullscreen = false;
                myVlcControl.Dock = DockStyle.None;
                menuStrip1.Visible = true;
                panelBottom.Visible = true;
            }
            else
            {
                this.MaximizeBox = false;
                this.WindowState = FormWindowState.Maximized;
                this.FormBorderStyle = FormBorderStyle.None;
                myVlcControl.Dock = DockStyle.Fill;
                menuStrip1.Visible = false;
                panelBottom.Visible = false;
                isFullscreen = true;
            }
        }


        private void button_fullscreen_Click(object sender, EventArgs e)
        {
            fullscreen();
        }


        private void panel1_DoubleClick(object sender, EventArgs e)
        {
            fullscreen();
        }


        private void MainWindow_DoubleClick(object sender, EventArgs e)
        {
            fullscreen();
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                fullscreen();
            else if(e.KeyCode == Keys.Space)
            {
                // pause/start/resume playback function call
            }
        }

        private void panelTime_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panelBottom_MouseEnter(object sender, EventArgs e)
        {
            if (isFullscreen) panelBottom.Visible = true;
        }

        private void panelBottom_MouseLeave(object sender, EventArgs e)
        {
            if (isFullscreen) panelBottom.Visible = false;
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            if (isFullscreen)
            {
                panelBottom.Visible = !panelBottom.Visible;
                menuStrip1.Visible = !menuStrip1.Visible;
            }
        }
    }

}
