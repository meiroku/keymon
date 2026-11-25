using System.Runtime.InteropServices;

namespace Keymon;

/// <summary>
/// Win32 APIのP/Invoke宣言
/// </summary>
internal static partial class NativeMethods
{
    /// <summary>低レベルキーボードフックの種類</summary>
    internal const int WhKeyboardLowLevel = 13;

    /// <summary>メッセージループを終了させるメッセージ</summary>
    internal const uint WmQuit = 0x0012;

    /// <summary>PeekMessageでキューからメッセージを取り除かない。</summary>
    internal const uint PmNoRemove = 0x0000;

    /// <summary>Win32のPOINT</summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct Point
    {
        public int X;
        public int Y;
    }

    /// <summary>Win32のMSG</summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct Message
    {
        public nint WindowHandle;
        public uint MessageId;
        public nint WParam;
        public nint LParam;
        public uint Time;
        public Point Point;
    }

    /// <summary>
    /// フックプロシージャをフックチェーンへ登録する。
    /// 失敗時は0を返し、エラーコードは <see cref="Marshal.GetLastPInvokeError"/> で取得する。
    /// </summary>
    [LibraryImport("user32.dll", EntryPoint = "SetWindowsHookExW", SetLastError = true)]
    internal static unsafe partial nint SetWindowsHookEx(
        int hookType,
        delegate* unmanaged<int, nint, nint, nint> callback,
        nint moduleHandle,
        uint threadId
    );

    /// <summary>
    /// フックをフックチェーンから取り除く。
    /// </summary>
    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool UnhookWindowsHookEx(nint hookHandle);

    /// <summary>
    /// フックチェーンの次のフックプロシージャへ処理を渡す。
    /// </summary>
    [LibraryImport("user32.dll")]
    internal static partial nint CallNextHookEx(
        nint hookHandle,
        int code,
        nint wParam,
        nint lParam
    );

    /// <summary>
    /// 指定したスレッドのメッセージキューへメッセージを投稿する。
    /// </summary>
    [LibraryImport("user32.dll", EntryPoint = "PostThreadMessageW", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool PostThreadMessage(
        uint threadId,
        uint message,
        nint wParam,
        nint lParam
    );

    /// <summary>
    /// メッセージを取得する。WM_QUITで0、エラーで-1、それ以外は0以外を返す。
    /// </summary>
    [LibraryImport("user32.dll", EntryPoint = "GetMessageW", SetLastError = true)]
    internal static partial int GetMessage(
        out Message message,
        nint windowHandle,
        uint minimumMessage,
        uint maximumMessage
    );

    /// <summary>
    /// メッセージを覗き見る。初回呼び出しで、呼び出しスレッドのメッセージキューが作成される。
    /// </summary>
    [LibraryImport("user32.dll", EntryPoint = "PeekMessageW")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool PeekMessage(
        out Message message,
        nint windowHandle,
        uint minimumMessage,
        uint maximumMessage,
        uint removeMessage
    );

    /// <summary>
    /// 呼び出しスレッドの識別子を取得する。
    /// </summary>
    [LibraryImport("kernel32.dll")]
    internal static partial uint GetCurrentThreadId();
}
