﻿using System;
using Caliburn.Micro;

namespace NZazuFiddle
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FiddleViewModel : Screen, IFiddle
    {
        public IFormDefinitionViewModel Definition { get; private set; }
        public IFormDataViewModel Data { get; private set; }
        public IPreviewViewModel Preview { get; private set; }

        public FiddleViewModel(IFormDefinitionViewModel definition,
            IFormDataViewModel data,
            IPreviewViewModel preview)
        {
            if (definition == null) throw new ArgumentNullException("definition");
            if (data == null) throw new ArgumentNullException("data");
            if (preview == null) throw new ArgumentNullException("preview");
            Definition = definition;
            Data = data;
            Preview = preview;
        }
    }
}