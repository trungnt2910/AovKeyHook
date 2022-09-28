using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace AovKeyHook
{
    static class TouchSimulator
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool InitializeTouchInjection(uint maxCount, uint dwMode);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool InjectTouchInput(uint count, ref PointerTouchInfo contacts);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterPointerInputTarget(IntPtr hwnd, PointerInputType pointerType);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        const uint TOUCH_FEEDBACK_DEFAULT = 0x1;

        static TouchSimulator()
        {
            Debug.Assert(InitializeTouchInjection(2, TOUCH_FEEDBACK_DEFAULT));
        }

        public static void SimulateTap(IntPtr hwnd, Point point, uint id)
        {
            //if (!RegisterPointerInputTarget(hwnd, PointerInputType.Touch))
            //{
            //    throw new Win32Exception(Marshal.GetLastWin32Error());
            //}

            GetWindowRect(hwnd, out RECT rect);

            PointerTouchInfo contact = new PointerTouchInfo();

            Console.WriteLine(Marshal.SizeOf<PointerTouchInfo>());
            Console.WriteLine(Marshal.SizeOf<PointerInfo>());

            contact.pointerInfo.pointerType = PointerInputType.Touch;
            contact.pointerInfo.pointerId = id;          //contact 0
            contact.pointerInfo.ptPixelLocation.y = (int)point.Y + rect.top; // Y co-ordinate of touch on screen
            contact.pointerInfo.ptPixelLocation.x = (int)point.X + rect.left; // X co-ordinate of touch on screen

            contact.touchFlags = TouchFlags.None;
            contact.touchMask = TouchMask.ContactArea | TouchMask.Orientation | TouchMask.Pressure;
            contact.orientation = 90; // Orientation of 90 means touching perpendicular to screen.
            contact.pressure = 32000;

            contact.rcContact.top = contact.pointerInfo.ptPixelLocation.y - 2;
            contact.rcContact.bottom = contact.pointerInfo.ptPixelLocation.y + 2;
            contact.rcContact.left = contact.pointerInfo.ptPixelLocation.x - 2;
            contact.rcContact.right = contact.pointerInfo.ptPixelLocation.x + 2;

            contact.pointerInfo.pointerFlags = PointerFlags.Down | PointerFlags.InRange | PointerFlags.InContact;

            if (!InjectTouchInput(1, ref contact))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            contact.pointerInfo.pointerFlags = PointerFlags.Up;

            if (!InjectTouchInput(1, ref contact))
            {
                Console.WriteLine(new Win32Exception(Marshal.GetLastWin32Error()));
            }
        }

        public static PointerTouchInfo InitDrag(IntPtr hwnd, Point point, uint id)
        {
            GetWindowRect(hwnd, out RECT rect);

            PointerTouchInfo contact = new PointerTouchInfo();

            Console.WriteLine(Marshal.SizeOf<PointerTouchInfo>());
            Console.WriteLine(Marshal.SizeOf<PointerInfo>());

            contact.pointerInfo.pointerType = PointerInputType.Touch;
            contact.pointerInfo.pointerId = id;          //contact 0
            contact.pointerInfo.ptPixelLocation.y = (int)point.Y + rect.top; // Y co-ordinate of touch on screen
            contact.pointerInfo.ptPixelLocation.x = (int)point.X + rect.left; // X co-ordinate of touch on screen

            contact.touchFlags = TouchFlags.None;
            contact.touchMask = TouchMask.ContactArea | TouchMask.Orientation | TouchMask.Pressure;
            contact.orientation = 90; // Orientation of 90 means touching perpendicular to screen.
            contact.pressure = 32000;

            contact.rcContact.top = contact.pointerInfo.ptPixelLocation.y - 2;
            contact.rcContact.bottom = contact.pointerInfo.ptPixelLocation.y + 2;
            contact.rcContact.left = contact.pointerInfo.ptPixelLocation.x - 2;
            contact.rcContact.right = contact.pointerInfo.ptPixelLocation.x + 2;

            contact.pointerInfo.pointerFlags = PointerFlags.Down | PointerFlags.InRange | PointerFlags.InContact;

            if (!InjectTouchInput(1, ref contact))
            {
                Console.WriteLine(new Win32Exception(Marshal.GetLastWin32Error()));
            }

            return contact;
        }

        public static void UpdateDrag(ref PointerTouchInfo contact, IntPtr hwnd, Point point)
        {
            GetWindowRect(hwnd, out RECT rect);

            contact.pointerInfo.ptPixelLocation.y = (int)point.Y + rect.top; // Y co-ordinate of touch on screen
            contact.pointerInfo.ptPixelLocation.x = (int)point.X + rect.left; // X co-ordinate of touch on screen

            contact.rcContact.top = contact.pointerInfo.ptPixelLocation.y - 2;
            contact.rcContact.bottom = contact.pointerInfo.ptPixelLocation.y + 2;
            contact.rcContact.left = contact.pointerInfo.ptPixelLocation.x - 2;
            contact.rcContact.right = contact.pointerInfo.ptPixelLocation.x + 2;

            contact.pointerInfo.pointerFlags = PointerFlags.Update | PointerFlags.InRange | PointerFlags.InContact;

            if (!InjectTouchInput(1, ref contact))
            {
                Console.WriteLine(new Win32Exception(Marshal.GetLastWin32Error()));
            }
        }

        public static void EndDrag(ref PointerTouchInfo contact, IntPtr hwnd, Point point)
        {
            GetWindowRect(hwnd, out RECT rect);

            contact.pointerInfo.ptPixelLocation.y = (int)point.Y + rect.top; // Y co-ordinate of touch on screen
            contact.pointerInfo.ptPixelLocation.x = (int)point.X + rect.left; // X co-ordinate of touch on screen

            contact.rcContact.top = contact.pointerInfo.ptPixelLocation.y - 2;
            contact.rcContact.bottom = contact.pointerInfo.ptPixelLocation.y + 2;
            contact.rcContact.left = contact.pointerInfo.ptPixelLocation.x - 2;
            contact.rcContact.right = contact.pointerInfo.ptPixelLocation.x + 2;

            contact.pointerInfo.pointerFlags = PointerFlags.Up;

            if (!InjectTouchInput(1, ref contact))
            {
                Console.WriteLine(new Win32Exception(Marshal.GetLastWin32Error()));
            }
        }
    }
}
