using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace AovKeyHook
{
    static class WinApiTouch
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool InitializeTouchInjection(uint maxCount, uint dwMode = TOUCH_FEEDBACK_DEFAULT);

        public static bool InjectTouchInput(ref PointerTouchInfo contacts) => InjectTouchInput(1, ref contacts);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool InjectTouchInput(uint count, ref PointerTouchInfo contacts);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool InjectTouchInput(uint count, [MarshalAs(UnmanagedType.LPArray)] PointerTouchInfo[] contacts);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterPointerInputTarget(IntPtr hwnd, PointerInputType pointerType);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr CreateSyntheticPointerDevice(PointerInputType pointerType, ulong maxCount, uint mode = POINTER_FEEDBACK_DEFAULT);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool InjectSyntheticPointerInput(IntPtr device, PointerTypeInfo[] pointerInfo, uint count);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool InjectSyntheticPointerInput(IntPtr device, ref PointerTypeInfo pointerInfo, uint count = 1);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        public const uint POINTER_FEEDBACK_DEFAULT = 0x1;
        public const uint TOUCH_FEEDBACK_DEFAULT = 0x1;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int x;
        public int y;

        public override string ToString()
        {
            return $"{{{x},{y}}}";
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct PointerTypeInfo
    {
        [FieldOffset(0)]
        public PointerInputType type;
        [FieldOffset(sizeof(PointerInputType))]
        public PointerTouchInfo touchInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PointerTouchInfo
    {
        public PointerInfo pointerInfo;
        public TouchFlags touchFlags;
        public TouchMask touchMask;
        public RECT rcContact;
        public RECT rcContactRaw;
        public uint orientation;
        public uint pressure;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PointerInfo
    {
        public PointerInputType pointerType;
        public uint pointerId;
        public uint frameId;
        public PointerFlags pointerFlags;
        public IntPtr sourceDevice;
        public IntPtr hwndTarget;
        public POINT ptPixelLocation;
        public POINT ptHimetricLocation;
        public POINT ptPixelLocationRaw;
        public POINT ptHimetricLocationRaw;
        public uint dwTime;
        public uint historyCount;
        public int InputData;
        public uint dwKeyStates;
        public ulong PerformanceCount;
        public PointerButtonChangeType ButtonChangeType;
    }

    public enum PointerInputType : uint
    {
        Pointer = 1,
        Touch,
        Pen,
        Mouse,
        TouchPad
    }

    [Flags]
    public enum PointerFlags : uint
    {
        None = 0x0,
        InRange = 0x00000002,
        InContact = 0x00000004,
        Down = 0x00010000,
        Update = 0x00020000,
        Up = 0x00040000,
    }

    public enum PointerButtonChangeType : uint
    {
        None,
        FirstButtonDown,
        FirstButtonUp,
        POINTER_CHANGE_SECONDBUTTON_DOWN,
        POINTER_CHANGE_SECONDBUTTON_UP,
        POINTER_CHANGE_THIRDBUTTON_DOWN,
        POINTER_CHANGE_THIRDBUTTON_UP,
        POINTER_CHANGE_FOURTHBUTTON_DOWN,
        POINTER_CHANGE_FOURTHBUTTON_UP,
        POINTER_CHANGE_FIFTHBUTTON_DOWN,
        POINTER_CHANGE_FIFTHBUTTON_UP
    }

    [Flags]
    public enum TouchFlags : uint
    {
        None = 0x0,
    }

    [Flags]
    public enum TouchMask : uint
    {
        None = 0x0,
        ContactArea = 0x1,
        Orientation = 0x2,
        Pressure = 0x4
    }
}
