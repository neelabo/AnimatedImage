﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace AnimatedImage.Wpf
{
    /// <summary>
    /// Provides a way to pause, resume or seek a GIF animation.
    /// </summary>
    public class ImageAnimationController : IDisposable
    {
        private readonly Image _image;
        private readonly RendererAnimation _animation;
        private readonly AnimationClock _clock;
        private readonly ClockController _clockController;

        internal ImageAnimationController(Image image, RendererAnimation animation, bool autoStart)
        {
            _image = image;
            _clock = animation.CreateClock();

            _animation = _clock.Timeline as RendererAnimation ?? animation;
            _clock.Completed += AnimationCompleted;
            _animation.CurrentIndexUpdated += OnCurrentFrameChanged;

            _clockController = _clock.Controller;


            // ReSharper disable once PossibleNullReferenceException
            _clockController.Pause();

            _image.ApplyAnimationClock(Image.SourceProperty, _clock);

            IsPaused = !autoStart;
            if (autoStart)
                _clockController.Resume();
        }

        void AnimationCompleted(object? sender, EventArgs e)
        {
            _image.RaiseEvent(new System.Windows.RoutedEventArgs(ImageBehavior.AnimationCompletedEvent, _image));
        }

        /// <summary>
        /// Returns the number of frames in the image.
        /// </summary>
        public int FrameCount
        {
            get { return _animation.Count; }
        }

        /// <summary>
        /// Returns the duration of the animation.
        /// </summary>
        public TimeSpan Duration
        {
            get
            {
                return _animation.Duration.HasTimeSpan
                  ? _animation.Duration.TimeSpan
                  : TimeSpan.Zero;
            }
        }

        /// <summary>
        /// Returns a value that indicates whether the animation is paused.
        /// </summary>
        public bool IsPaused { get; private set; }

        /// <summary>
        /// Returns a value that indicates whether the animation is complete.
        /// </summary>
        public bool IsComplete
        {
            get { return _clock.CurrentState == ClockState.Filling; }
        }

        /// <summary>
        /// Seeks the animation to the specified frame index.
        /// </summary>
        /// <param name="index">The index of the frame to seek to</param>
        public void GotoFrame(int index)
        {
            var startTime = _animation.GetStartTime(index);
            _clockController.Seek(startTime, TimeSeekOrigin.BeginTime);
        }

        /// <summary>
        /// Returns the current frame index.
        /// </summary>
        public int CurrentFrame
        {
            get
            {
                return _animation.CurrentIndex;
            }
        }

        /// <summary>
        /// Pauses the animation.
        /// </summary>
        public void Pause()
        {
            IsPaused = true;
            _clockController.Pause();
        }

        /// <summary>
        /// Starts or resumes the animation. If the animation is complete, it restarts from the beginning.
        /// </summary>
        public void Play()
        {
            IsPaused = false;
            if (!_isSuspended)
                _clockController.Resume();
        }

        private bool _isSuspended;
        internal void SetSuspended(bool isSuspended)
        {
            if (isSuspended == _isSuspended)
                return;

            bool wasSuspended = _isSuspended;
            _isSuspended = isSuspended;
            if (wasSuspended)
            {
                if (!IsPaused)
                {
                    _clockController.Resume();
                }
            }
            else
            {
                _clockController.Pause();
            }
        }

        /// <summary>
        /// Raised when the current frame changes.
        /// </summary>
        public event EventHandler? CurrentFrameChanged;

        private void OnCurrentFrameChanged()
        {
            EventHandler? handler = CurrentFrameChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        /// <summary>
        /// Finalizes the current object.
        /// </summary>
        ~ImageAnimationController()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes the current object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the current object
        /// </summary>
        /// <param name="disposing">true to dispose both managed an unmanaged resources, false to dispose only managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _image.BeginAnimation(Image.SourceProperty, null);
                _clock.Completed -= AnimationCompleted;
                _animation.CurrentIndexUpdated -= OnCurrentFrameChanged;
                // Keep image after dispose 
                //_image.Source = null;
            }
        }
    }
}