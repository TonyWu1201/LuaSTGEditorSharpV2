﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using LuaSTGEditorSharpV2.Core;
using LuaSTGEditorSharpV2.Core.Model;
using LuaSTGEditorSharpV2.Services;
using LuaSTGEditorSharpV2.Core.Services;
using LuaSTGEditorSharpV2.WPF;
using System.Windows;
using LuaSTGEditorSharpV2.Core.Command;
using LuaSTGEditorSharpV2.Core.Command.Service;

namespace LuaSTGEditorSharpV2.ViewModel
{
    public class DocumentViewModel : DockingViewModelBase
    {
        public ObservableCollection<NodeViewModel> Tree { get; private set; } = [];

        private readonly EditingDocumentModel _editingDocumentModel;
        public IDocument DocumentModel => _editingDocumentModel;

        private string _rawTitle = string.Empty;
        public string RawTitle => _rawTitle;
        public override string Title =>
            string.Format("{0}{1}", _rawTitle, _editingDocumentModel.IsModified ? " *" : string.Empty);

        public bool IsModified => _editingDocumentModel.IsModified;

        public bool CanUndo => _editingDocumentModel.CanUndo;
        public bool CanRedo => _editingDocumentModel.CanRedo;

        public DocumentViewModel(EditingDocumentModel documentModel)
        {
            _editingDocumentModel = documentModel;
            _rawTitle = documentModel.FileName;
            Tree.Add(HostedApplicationHelper.GetService<ViewModelProviderServiceProvider>()
                .CreateViewModelRecursive(documentModel.Root, new LocalServiceParam(documentModel)));
        }

        /// <summary>
        /// If the document is already on disk, save the file.
        /// Otherwise ask user the path where the document should be saved.
        /// </summary>
        /// <returns> 
        /// <see cref="true"/> if Document has saved, otherwise <see cref="false"/>. 
        /// </returns>
        public bool SaveOrAskToSaveAs()
        {
            if (DocumentModel.IsOnDisk())
            {
                Save();
                return true;
            }
            else
            {
                return SaveAs();
            }
        }

        private void Save()
        {
            DocumentModel.Save();
            RaisePropertyChanged(nameof(Title));
        }

        /// <summary>
        /// Ask user the path where the document should be saved, then save the document to that directory.
        /// </summary>
        /// <returns> 
        /// <see cref="true"/> if Document has saved, otherwise <see cref="false"/>. 
        /// </returns>
        public bool SaveAs()
        {
            var fileDialog = HostedApplicationHelper.GetService<FileDialogService>();
            string? path = fileDialog.ShowSaveAsFileCommandDialog(DocumentModel.FileName);
            if (path == null) return false;
            DocumentModel.SaveAs(path);
            RaisePropertyChanged(nameof(Title));
            return true;
        }

        public void AskSaveBeforeClose()
        {
            var localization = HostedApplicationHelper.GetService<LocalizationService>();
            var messageBoxResult =
                MessageBox.Show(
                    string.Format(localization.GetString("messageBox_saveBeforClose_message",
                        typeof(WorkSpaceViewModel).Assembly), RawTitle),
                    localization.GetString("messageBox_title_app", typeof(WindowHelper).Assembly),
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Information
                    );
            if (messageBoxResult == MessageBoxResult.Cancel) return;
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                SaveOrAskToSaveAs();
            }
        }

        public void CloseActiveDocument()
        {
            var activeDocService = HostedApplicationHelper.GetService<ActiveDocumentService>();
            activeDocService.Close(_editingDocumentModel);
            activeDocService.MarkAsSaved(_editingDocumentModel);
        }

        public void ExecuteCommand(CommandBase command)
        {
            _editingDocumentModel.ExecuteCommand(command);
            RaisePropertyChanged(nameof(Title));
        }

        public void Undo()
        {
            _editingDocumentModel.Undo();
            RaisePropertyChanged(nameof(Title));
        }

        public void Redo()
        {
            _editingDocumentModel.Redo();
            RaisePropertyChanged(nameof(Title));
        }
    }
}
