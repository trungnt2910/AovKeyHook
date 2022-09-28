using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace AovKeyHook
{
    static class Fingers
    {
        static readonly PointerTouchInfo[] _arr = new PointerTouchInfo[2];
        static ref PointerTouchInfo _left => ref _arr[0];
        static ref PointerTouchInfo _right => ref _arr[1];
        static readonly DispatcherTimer _timer;

        static bool _isLeftDown;
        static bool _wasLeftDown = false;
        static bool _isRightDown;
        static bool _wasRightDown = false;

        static Point _leftLocation;
        static Point _rightLocation;
        static IntPtr _hwnd;

        static readonly IntPtr _device = WinApiTouch.CreateSyntheticPointerDevice(PointerInputType.Touch, 2, 0x2);

        static TaskCompletionSource<object> _tcs = new TaskCompletionSource<object>();

        static Fingers()
        {
            Debug.Assert(_device != IntPtr.Zero);

            //_timer = new DispatcherTimer();
            //_timer.Tick += OnTick;
            //_timer.Interval = TimeSpan.FromMilliseconds(0.1);

            PopulateInfo(ref _left);
            _left.pointerInfo.pointerId = 0;
            PopulateInfo(ref _right);
            _right.pointerInfo.pointerId = 1;

            //_left.pointerInfo.sourceDevice = _device;
            //_right.pointerInfo.sourceDevice = _device;

            WinApiTouch.InitializeTouchInjection(3);

            var t = new Thread(() =>
            {
                while (true)
                {
                    OnTick(null, EventArgs.Empty);
                }
            });

            t.Start();
        }

        public static void SetLeftStatus(Point? location, bool? isPressed)
        {
            _leftLocation = location ?? _leftLocation;
            _isLeftDown = isPressed ?? _isLeftDown;
        }

        public static void SetRightStatus(Point? location, bool? isPressed)
        {
            _rightLocation = location ?? _rightLocation;
            _isRightDown = isPressed ?? _isRightDown;
        }

        public static void SetWindow(IntPtr hwnd)
        {
            _hwnd = hwnd;
        }

        public static async Task WaitForNextFrame()
        {
            await _tcs.Task;
        }

        private static void PopulateInfo(ref PointerTouchInfo contact)
        {
            contact.pointerInfo.pointerType = PointerInputType.Touch;

            contact.touchFlags = TouchFlags.None;
            contact.touchMask = TouchMask.ContactArea | TouchMask.Orientation | TouchMask.Pressure;
            contact.orientation = 90; // Orientation of 90 means touching perpendicular to screen.
            contact.pressure = 32000;
        }

        private static void PopulateCoords(ref PointerTouchInfo contact, POINT point)
        {
            contact.pointerInfo.ptPixelLocation = point;

            contact.rcContact.top = contact.pointerInfo.ptPixelLocation.y - 2;
            contact.rcContact.bottom = contact.pointerInfo.ptPixelLocation.y + 2;
            contact.rcContact.left = contact.pointerInfo.ptPixelLocation.x - 2;
            contact.rcContact.right = contact.pointerInfo.ptPixelLocation.x + 2;
        }

        private static bool PopulateFlags(ref PointerTouchInfo contact, bool wasDown, bool isDown)
        {
            if (!wasDown && !isDown)
            {
                contact.pointerInfo.pointerFlags = PointerFlags.Up;
                return false;
            }

            if (!wasDown) // Was not down, is down
            {
                contact.pointerInfo.pointerFlags = PointerFlags.Down | PointerFlags.InContact | PointerFlags.InRange;
            }
            else if (isDown) // Was down, is down
            {
                contact.pointerInfo.pointerFlags = PointerFlags.Update | PointerFlags.InContact | PointerFlags.InRange;
            }
            else // Was down, is not down
            {
                contact.pointerInfo.pointerFlags = PointerFlags.Up;
            }

            return true;
        }

        private static void ThrowException()
        {
            var except = new Win32Exception(Marshal.GetLastWin32Error());
            Console.WriteLine(except);
        }

        private static void OnTick(object sender, EventArgs e)
        {
            WinApiTouch.GetWindowRect(_hwnd, out var rect);
            var leftPoint = new POINT()
            {
                x = rect.left + (int)_leftLocation.X,
                y = rect.top + (int)_leftLocation.Y
            };

            var rightPoint = new POINT()
            {
                x = rect.left + (int)_rightLocation.X,
                y = rect.top + (int)_rightLocation.Y
            };

            PopulateCoords(ref _left, leftPoint);
            PopulateCoords(ref _right, rightPoint);

            var shouldApplyLeft = PopulateFlags(ref _left, _wasLeftDown, _isLeftDown);
            _wasLeftDown = _isLeftDown;

            var shouldApplyRight = PopulateFlags(ref _right, _wasRightDown, _isRightDown);
            _wasRightDown = _isRightDown;

            //if (!WinApiTouch.InjectTouchInput(2, _arr))
            //{
            //    ThrowException();
            //}

            var left = new PointerTypeInfo()
            {
                type = PointerInputType.Touch,
                touchInfo = _left
            };

            var right = new PointerTypeInfo()
            {
                type = PointerInputType.Touch,
                touchInfo = _right
            };

            //Console.WriteLine($"L: ({_left.pointerInfo.pointerFlags}, {leftPoint})\nR: ({_right.pointerInfo.pointerFlags}, {rightPoint})");

            if (shouldApplyLeft && shouldApplyRight)
            {
                if (!WinApiTouch.InjectTouchInput(2, _arr))
                {
                    ThrowException();
                }
            }
            else if (shouldApplyLeft)
            {
                if (!WinApiTouch.InjectTouchInput(ref _left))
                {
                    ThrowException();
                }
            }
            else if (shouldApplyRight)
            {
                if (!WinApiTouch.InjectTouchInput(ref _right))
                {
                    ThrowException();
                }
            }

            _tcs.SetResult(null);
            _tcs = new TaskCompletionSource<object>();
        }
    }
}
