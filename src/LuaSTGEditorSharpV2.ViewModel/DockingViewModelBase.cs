﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using CommunityToolkit.Mvvm.Input;

namespace LuaSTGEditorSharpV2.ViewModel
{
    /// <summary>
    /// Base viewmodel for any docking panels
    /// </summary>
    public abstract class DockingViewModelBase : ViewModelBase
    {
        public event EventHandler? OnClose;
        public event EventHandler? OnReopen;

        private ICommand? _closeCommand;
        public ICommand CloseCommand
        {
            get
            {
                _closeCommand ??= new RelayCommand(Close);
                return _closeCommand;
            }
        }

        private bool _canClose = true;
        public bool CanClose
        {
            get { return _canClose; }
            set
            {
                if (_canClose != value)
                {
                    _canClose = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    if (_isActive)
                    {
                        Reopen();
                    }
                    else
                    {
                        Close();
                    }
                    RaisePropertyChanged();
                }
            }
        }

        public void Close()
        {
            OnClose?.Invoke(this, EventArgs.Empty);
        }

        public void Reopen()
        {
            OnReopen?.Invoke(this, EventArgs.Empty);
        }

        public abstract string Title { get; }
    }
}