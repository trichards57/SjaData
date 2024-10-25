// -----------------------------------------------------------------------
// <copyright file="ConsoleFunctions.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Runtime.InteropServices;

namespace AODashboard.VorUploader;

/// <summary>
/// Helper functions to get console details.
/// </summary>
internal static partial class ConsoleFunctions
{
    /// <summary>
    /// Controls the ancestor returned.
    /// </summary>
    private enum GetAncestorControl
    {
        /// <summary>
        /// Gets the immediate parent window.
        /// </summary>
        GetParent = 1,

        /// <summary>
        /// Gets the root window.
        /// </summary>
        GetRoot = 2,

        /// <summary>
        /// Retrieves the owned root window by walking the chain of parent and owner windows returned by GetParent.
        /// </summary>
        GetRootOwner = 3,
    }

    /// <summary>
    /// Gets the window handle for the current console or terminal.
    /// </summary>
    /// <returns>
    /// The window handle.
    /// </returns>
    public static IntPtr GetConsoleOrTerminalWindow()
    {
        var consoleHandle = GetConsoleWindow();
        var handle = GetAncestor(consoleHandle, GetAncestorControl.GetRootOwner);

        return handle;
    }

    [LibraryImport("user32.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    private static partial IntPtr GetAncestor(IntPtr hwnd, GetAncestorControl flags);

    [LibraryImport("kernel32.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    private static partial IntPtr GetConsoleWindow();
}
