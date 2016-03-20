// -----------------------------------------------------------------------
// <copyright file="ProgressBarViewModel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CAT
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;

    /// <summary>
    /// View Model class for ProgressBarWindow
    /// </summary>
    public class ProgressBarViewModel : INotifyPropertyChanged, IProgressBar
    {
        /// <summary>
        /// ProgressBarWindow UI element
        /// </summary>
        private ProgressBarWindow progressBarWindow;

        /// <summary>
        /// Delegate to invoke when progress bar is displayed
        /// </summary>
        private ProgressDelegate progressDelegate;

        /// <summary>
        /// Indicates whether or not the progress bar has been started yet
        /// </summary>
        private bool started;

        /// <summary>
        /// Maximum value of the progress bar
        /// </summary>
        private uint max = uint.MaxValue;

        /// <summary>
        /// Current value of the progress bar
        /// </summary>
        private uint progress;

        /// <summary>
        /// Backing property for AnimationBrush
        /// </summary>
        private Brush animationBrush;

        /// <summary>
        /// All animation brushes
        /// </summary>
        private Brush[] animationBrushes = new Brush[4];

        /// <summary>
        /// Index into animationBrushes of the current animation frame
        /// </summary>
        private uint brushIndex;

        /// <summary>
        /// Time of the last new frame
        /// </summary>
        private DateTime lastFrame = DateTime.Now - TimeSpan.FromDays(1);

        /// <summary>
        /// Timer used to update the animation frame
        /// </summary>
        private Timer animationTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressBarViewModel" /> class.
        /// </summary>
        /// <param name="title">Title for the progress bar</param>
        /// <param name="parentWindow">parent window of the progress bar.  Progress bar is centered within the parent window</param>
        public ProgressBarViewModel(string title, Window parentWindow)
        {
            for (uint brushIndex = 0; brushIndex < 4; brushIndex++)
            {
                string fileName = "cat0" + (6 + brushIndex) + "small.png";
                BitmapImage bitmapImage = new BitmapImage(new Uri("pack://application:,,,/Images/" + fileName));
                ImageBrush imageBrush = new ImageBrush(bitmapImage);
                imageBrush.AlignmentX = AlignmentX.Right;
                imageBrush.Stretch = Stretch.None;
                this.animationBrushes[brushIndex] = imageBrush;
            }

            this.Title = title;
            this.progressBarWindow = new ProgressBarWindow();
            this.progressBarWindow.Owner = parentWindow;
            this.progressBarWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            this.progressBarWindow.Loaded += this.OnLoad;

            FrameworkElement element = this.progressBarWindow as FrameworkElement;
            element.DataContext = this;

            this.animationTimer = new Timer(_ => this.AnimationTimerExpired(), null, TimeSpan.FromMilliseconds(200), TimeSpan.FromMilliseconds(-1));
        }

        /// <summary>
        /// PropertyChanged event used by NotifyPropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the maximum value of the progress bar
        /// </summary>
        public uint Max
        {
            get
            {
                return this.max;
            }

            set
            {
                this.max = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the title of the progress bar
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Sets the delegate called when the progress bar is started
        /// </summary>
        public ProgressDelegate Delegate
        {
            set { this.progressDelegate = value; }
        }

        /// <summary>
        /// Gets or sets the current progress of the progress bar
        /// </summary>
        public uint Progress
        {
            get
            {
                return this.progress;
            }

            set
            {
                this.progress = value;
                this.NotifyPropertyChanged("Progress");
            }
        }

        /// <summary>
        /// Gets or sets the current animation brush
        /// </summary>
        public Brush AnimationBrush
        {
            get
            {
                return this.animationBrush;
            }

            set
            {
                this.animationBrush = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Shows the progress bar window
        /// </summary>
        public void Show()
        {
            this.progressBarWindow.ShowDialog();
        }

        /// <summary>
        /// Handler called when the progress bar window is loaded
        /// </summary>
        /// <param name="sender">Originator of the load event</param>
        /// <param name="e">An instance of RoutedEventArgs</param>
        public void OnLoad(object sender, RoutedEventArgs e)
        {
            if (!this.started)
            {
                this.started = true;
                Thread th = new Thread(new ParameterizedThreadStart(delegate
                {
                    if (this.progressDelegate != null)
                    {
                        this.progressDelegate();
                    }

                    this.progressBarWindow.Dispatcher.Invoke(new Action(() => { this.progressBarWindow.Close(); }), DispatcherPriority.ApplicationIdle);
                }));
                th.Start();
            }
        }

        /// <summary>
        /// NotifyPropertyChanged triggers the PropertyChanged event for the specified property
        /// </summary>
        /// <param name="propertyName">Name of property that has changed</param>
        private void NotifyPropertyChanged([CallerMemberName]string propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Timer expired callback in UI thread, updates the animation frame
        /// </summary>
        private void AnimationTimerExpiredInUIThread()
        {
            this.brushIndex++;
            if (this.brushIndex == 4)
            {
                this.brushIndex = 0;
            }

            Brush nextBrush = this.animationBrushes[this.brushIndex];
            this.AnimationBrush = nextBrush;

            this.animationTimer = new Timer(_ => this.AnimationTimerExpired(), null, TimeSpan.FromMilliseconds(200), TimeSpan.FromMilliseconds(-1));
        }

        /// <summary>
        /// Timer expired callback, updates the animation frame
        /// </summary>
        private void AnimationTimerExpired()
        {
            try
            {
                Dispatcher dispatcher = this.progressBarWindow.Dispatcher;
                if (dispatcher != null)
                {
                    dispatcher.BeginInvoke(new Action(() => { this.AnimationTimerExpiredInUIThread(); }));
                }
            }
            catch (TaskCanceledException)
            {
                // Don't complain about late arrival if UI has already gone away
            }
        }
    }
}
