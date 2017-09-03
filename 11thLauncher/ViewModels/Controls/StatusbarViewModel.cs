using System;
using System.Collections.Generic;
using Caliburn.Micro;
using _11thLauncher.Messages;
using _11thLauncher.Models;
using _11thLauncher.Util;

namespace _11thLauncher.ViewModels.Controls
{
    public class StatusbarViewModel : PropertyChangedBase, IHandle<UpdateStatusBarMessage>
    {
        #region Fields

        private readonly Dictionary<AsyncAction, int> _actions;
        private string _statusText;
        private bool _taskRunning;

        #endregion

        public StatusbarViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.Subscribe(this);
            _actions = new Dictionary<AsyncAction, int>();
            foreach (AsyncAction type in Enum.GetValues(typeof(AsyncAction)))
            {
                _actions[type] = 0;
            }
        }

        #region Properties

        public string StatusText
        {
            get => _statusText;
            set
            {
                _statusText = value;
                NotifyOfPropertyChange(() => StatusText);
            }
        }

        public bool TaskRunning
        {
            get => _taskRunning;
            set
            {
                _taskRunning = value;
                NotifyOfPropertyChange(() => TaskRunning);
            }
        }

        #endregion

        #region Message handling

        public void Handle(UpdateStatusBarMessage message)
        {
            if (message.IsRunning)
            {
                _actions[message.Action]++;
            }
            else
            {
                _actions[message.Action]--;
            }

            bool running = false;
            var statusText = string.Empty;
            foreach (KeyValuePair<AsyncAction, int> asyncAction in _actions)
            {
                if (asyncAction.Value <= 0) continue;
                running = true;

                statusText += asyncAction.Key.GetDescription() +
                              (asyncAction.Value > 1 ? " (x" + asyncAction.Value + ")" : string.Empty) + 
                              ", ";
            }

            if (running)
            {
                StatusText = statusText.Remove(statusText.Length - 2);
            }
            else
            {
                StatusText = StatusText = Resources.Strings.S_STATUS_READY;
            }

            TaskRunning = running;
        }

        #endregion
    }
}
