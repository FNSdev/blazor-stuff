using System;
using System.Threading;

namespace hephaestus.Services
{
    public class ToastService : IDisposable
    {
        public event Action<string, ToastLevel> OnShow;
        public event Action OnHide;
        private Timer Countdown;

        public void ShowToast(string message, ToastLevel level)
        {
            OnShow?.Invoke(message, level);
            StartCountdown();
        }

        private void StartCountdown()
        {
            SetCountdown();
        }

        private void SetCountdown()
        {
            if (Countdown == null)
            {
                Countdown = new Timer(HideToast, new AutoResetEvent(false), 3000, 3000);
            }
        }

        private void HideToast(object source)
        {
            OnHide?.Invoke();
        }

        public void Dispose()
        {
            Countdown?.Dispose();
        }
    }
    
}