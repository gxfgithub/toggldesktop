using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using ValidationHelper = ReactiveUI.Validation.Helpers.ValidationHelper;
using static TogglDesktop.MessageBox;

namespace TogglDesktop.ViewModels
{
    public class PreferencesWindowViewModel : ReactiveValidationObject<PreferencesWindowViewModel>
    {
        private readonly ShowMessageBoxDelegate _showMessageBox;
        private readonly Action _closePreferencesWindow;

        private readonly Dictionary<HotKey, string> _knownShortcuts =
            new Dictionary<HotKey, string>
            {
                { new HotKey(Key.N, ModifierKeys.Control), "New time entry" },
                { new HotKey(Key.O, ModifierKeys.Control), "Continue last entry" },
                { new HotKey(Key.S, ModifierKeys.Control), "Stop time entry" },
                { new HotKey(Key.W, ModifierKeys.Control), "Hide Toggl Desktop" },
                { new HotKey(Key.R, ModifierKeys.Control), "Sync" },
                { new HotKey(Key.E, ModifierKeys.Control), "Edit time entry" },
                { new HotKey(Key.D, ModifierKeys.Control), "Toggle manual mode" },
                { new HotKey(Key.V, ModifierKeys.Control), "New time entry from clipboard" },
            };

        private const string ShowHideTogglDescription = "Show/Hide Toggl";
        private const string ContinueStopTimerDescription = "Continue/Stop Timer";

        private HotKey _continueStopTimerSaved;
        private HotKey _showHideTogglSaved;
        private String _proxyHostSaved;

        private readonly ValidationHelper _showHideTogglValidation;
        private readonly ValidationHelper _continueStopTimerValidation;
        private readonly ValidationHelper _proxyHostValidation;

        public PreferencesWindowViewModel(ShowMessageBoxDelegate showMessageBox, Action closePreferencesWindow)
        {
            _showMessageBox = showMessageBox;
            _closePreferencesWindow = closePreferencesWindow;
            var isLoggedIn = Observable.FromEvent<Toggl.DisplayLogin, bool>(
                onNext => (open, userId) => { onNext(userId != 0); },
                x => Toggl.OnLogin += x,
                x => Toggl.OnLogin -= x);
            ClearCacheCommand = ReactiveCommand.Create(ClearCache, isLoggedIn.ObserveOnDispatcher());

            this.WhenAnyValue(x => x.ShowHideToggl)
                .Buffer(2, 1)
                .Subscribe(b => UpdateKnownShortcuts(b[0], b[1], ShowHideTogglDescription));
            this.WhenAnyValue(x => x.ContinueStopTimer)
                .Buffer(2, 1)
                .Subscribe(b => UpdateKnownShortcuts(b[0], b[1], ContinueStopTimerDescription));

            _showHideTogglValidation = this.ValidationRule(
                vm => vm.ShowHideToggl,
                hotKey => IsHotKeyValid(hotKey, ShowHideTogglDescription),
                hotKey => $"This shortcut is already taken by {_knownShortcuts[hotKey]}");
            _continueStopTimerValidation = this.ValidationRule(
                vm => vm.ContinueStopTimer,
                hotKey => IsHotKeyValid(hotKey, ContinueStopTimerDescription),
                hotKey => $"This shortcut is already taken by {_knownShortcuts[hotKey]}");
            _proxyHostValidation = this.ValidationRule(
                x => x.ProxyHost,
                proxyHost => Uri.CheckHostName(proxyHost) != UriHostNameType.Unknown,
                "Please, enter a valid host");
        }

        public ICommand ClearCacheCommand { get; }

        [Reactive]
        public HotKey ShowHideToggl { get; set; }
        public HotKey GetShowHideTogglIfChanged() =>
            !object.Equals(ShowHideToggl, _showHideTogglSaved) ? (ShowHideToggl ?? new HotKey(Key.None)) : null;

        [Reactive]
        public HotKey ContinueStopTimer { get; set; }
        public HotKey GetContinueStopTimerIfChanged() =>
            !object.Equals(ContinueStopTimer, _continueStopTimerSaved) ? (ContinueStopTimer ?? new HotKey(Key.None)) : null;

        [Reactive]
        public string ProxyHost { get; set; }

        public void ResetRecordedShortcuts()
        {
            ShowHideToggl = _showHideTogglSaved;
            ContinueStopTimer = _continueStopTimerSaved;
            ProxyHost = _proxyHostSaved;
        }

        public void LoadShortcutsFromSettings()
        {
            _showHideTogglSaved = LoadHotKey(Toggl.GetKeyShow, Toggl.GetKeyModifierShow);
            ShowHideToggl = _showHideTogglSaved;
            _continueStopTimerSaved = LoadHotKey(Toggl.GetKeyStart, Toggl.GetKeyModifierStart);
            ContinueStopTimer = _continueStopTimerSaved;
        }

        public void SetSavedProxyHost(String savedProxyHost)
        {
            _proxyHostSaved = savedProxyHost;
        }

        public void ResetPropsWithErrorsToPreviousValues()
        {
            if (!_showHideTogglValidation.IsValid) ShowHideToggl = _showHideTogglSaved;
            if (!_continueStopTimerValidation.IsValid) ContinueStopTimer = _continueStopTimerSaved;
            if (!_proxyHostValidation.IsValid) ProxyHost = _proxyHostSaved;
        }

        private bool IsHotKeyValid(HotKey hotKey, string hotKeyDescription)
        {
            return hotKey.IsNullOrNone() || _knownShortcuts[hotKey] == hotKeyDescription;
        }

        private void UpdateKnownShortcuts(HotKey previousValue, HotKey newValue, string hotKeyDescription)
        {
            if (!previousValue.IsNullOrNone() && _knownShortcuts.ContainsKey(previousValue))
            {
                if (_knownShortcuts[previousValue] == hotKeyDescription)
                {
                    _knownShortcuts.Remove(previousValue);
                }
            }
            if (!newValue.IsNullOrNone())
            {
                if (!_knownShortcuts.ContainsKey(newValue))
                {
                    _knownShortcuts.Add(newValue, hotKeyDescription);
                }
            }
        }

        private static HotKey LoadHotKey(Func<Key> getKey, Func<ModifierKeys> getModifiers)
        {
            var key = getKey();
            return new HotKey(key, key == Key.None ? ModifierKeys.None : getModifiers());
        }

        private void ClearCache()
        {
            var result = _showMessageBox(
                "This will remove your Toggl user data from this PC and log you out of the Toggl Desktop app. " +
                "Any unsynced data will be lost.\n\nDo you want to continue?", "Clear Cache",
                MessageBoxButton.OKCancel, "Clear cache");

            if (result == MessageBoxResult.OK)
            {
                Toggl.ClearCache();
                _closePreferencesWindow();
            }
        }
    }
}