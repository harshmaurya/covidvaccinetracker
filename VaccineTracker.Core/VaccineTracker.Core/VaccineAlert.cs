using System;
using System.Threading;
using System.Threading.Tasks;
using VaccineTracker.Core.Domain;

namespace VaccineTracker.Core
{
    public class VaccineAlert : IDisposable
    {
        private readonly Criteria _criteria;
        private IDisposable _currentSubscription;
        private readonly IAlertHandler[] _alertHandlers;

        public VaccineAlert(Criteria criteria, params IAlertHandler[] alertHandler)
        {
            _criteria = criteria;
            _alertHandlers = alertHandler;
        }

        public async Task Start()
        {
            var listener = new Listener();
            var observable = await listener.SubscribeAvailableSessions(_criteria);
            await Subscribe(_alertHandlers, observable);
        }

        public void Stop()
        {
            _currentSubscription?.Dispose();
        }

        private async Task Subscribe(IAlertHandler[] alertHandler, IObservable<VaccineAvailability> observable)
        {
            await Task.Factory.StartNew(() =>
            {
                _currentSubscription?.Dispose();
                _currentSubscription = observable.Subscribe(session =>
                {
                    foreach (var handler in alertHandler)
                    {
                        handler.HandleVaccineAvailability(session);
                    }
                }, async exception =>
                {
                    foreach (var handler in alertHandler)
                    {
                        handler.HandlerError(exception);
                    }
                    Thread.Sleep(TimeSpan.FromSeconds(10));
                    await Subscribe(alertHandler, observable);
                });
            });
        }

        public void Dispose()
        {
            _currentSubscription?.Dispose();
        }
    }
}