using System.Collections.Generic;
using Caliburn.Micro;
using _11thLauncher.Messages;
using _11thLauncher.Model;

namespace _11thLauncher.ViewModels.Controls
{
    public class StatusbarViewModel : PropertyChangedBase, IHandle<UpdateStatusBarMessage>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly Dictionary<AsyncAction, int> _actions = new Dictionary<AsyncAction, int>();
        private string _statusText = Resources.Strings.S_STATUS_READY;
        private bool _taskRunning;

        public StatusbarViewModel(IEventAggregator eventAggregator) //TODO this functionality
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
        }

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

        #region Message handling

        public void Handle(UpdateStatusBarMessage message)
        {
            if (message.IsRunning)
            {
                if (_actions.ContainsKey(message.Action))
                {
                    _actions[message.Action]++;
                }
                else
                {
                    _actions[message.Action] = 1;
                }
            }
            else
            {
                _actions[message.Action]--;
            }

            bool running = false;
            var statusText = "";
            foreach (KeyValuePair<AsyncAction, int> actionStatus in _actions)
            {
                if (actionStatus.Value == 0) continue;
                running = true;

                statusText += actionStatus.Key.GetDescription() +
                              (actionStatus.Value > 1 ? " (x" + actionStatus.Value + ")" : "") + 
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
