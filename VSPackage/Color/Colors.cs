﻿
namespace OpenCppCoverage.VSPackage.VSColor
{
    using System;
    using Microsoft.VisualStudio.Shell;

    internal sealed class VSColors
    {
        #region Autogenerated resource keys
        // These resource keys are generated by Visual Studio Extension Color Editor, and should be replaced when new colors are added to this category.
        public static readonly Guid Category = new Guid("77cc3bec-cf72-42fa-8b0c-47e8f1bbbbbd");

        private static ThemeResourceKey _CodeCoveredColorKey;
        private static ThemeResourceKey _CodeCoveredBrushKey;
        public static ThemeResourceKey CodeCoveredColorKey { get { return _CodeCoveredColorKey ?? (_CodeCoveredColorKey = new ThemeResourceKey(Category, "Code Covered", ThemeResourceKeyType.BackgroundColor)); } }
        public static ThemeResourceKey CodeCoveredBrushKey { get { return _CodeCoveredBrushKey ?? (_CodeCoveredBrushKey = new ThemeResourceKey(Category, "Code Covered", ThemeResourceKeyType.BackgroundBrush)); } }

        private static ThemeResourceKey _CodeNotCoveredColorKey;
        private static ThemeResourceKey _CodeNotCoveredBrushKey;
        public static ThemeResourceKey CodeNotCoveredColorKey { get { return _CodeNotCoveredColorKey ?? (_CodeNotCoveredColorKey = new ThemeResourceKey(Category, "Code Not Covered", ThemeResourceKeyType.BackgroundColor)); } }
        public static ThemeResourceKey CodeNotCoveredBrushKey { get { return _CodeNotCoveredBrushKey ?? (_CodeNotCoveredBrushKey = new ThemeResourceKey(Category, "Code Not Covered", ThemeResourceKeyType.BackgroundBrush)); } }
        #endregion
    }
}
