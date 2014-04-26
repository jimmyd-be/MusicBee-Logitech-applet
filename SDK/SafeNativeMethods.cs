using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;

namespace GammaJul.LgLcd {

	internal static class SafeNativeMethods {
		private const int ErrorSuccess = 0;
		private const int ErrorAccessDenied = 5;
		private const int ErrorDeviceNotConnected = 1167;


		#region Initialization

		[SuppressUnmanagedCodeSecurity]
		[DllImport("GammaJul.LgLcd.Native32.dll", CharSet = CharSet.Auto, EntryPoint = "lgLcdInit", ExactSpelling = true)]
		private static extern int LgLcdInit32();

		[SuppressUnmanagedCodeSecurity]
		[DllImport("GammaJul.LgLcd.Native64.dll", CharSet = CharSet.Auto, EntryPoint = "lgLcdInit", ExactSpelling = true)]
		private static extern int LgLcdInit64();

		internal static void LgLcdInit() {
			int result = IntPtr.Size == 8
				? LgLcdInit64()
				: LgLcdInit32();
			if (result != ErrorSuccess)
				throw new Win32Exception(result);
		}


		[SuppressUnmanagedCodeSecurity]
		[DllImport("GammaJul.LgLcd.Native32.dll", CharSet = CharSet.Auto, EntryPoint = "lgLcdDeInit", ExactSpelling = true)]
		private static extern int LgLcdDeInit32();

		[SuppressUnmanagedCodeSecurity]
		[DllImport("GammaJul.LgLcd.Native64.dll", CharSet = CharSet.Auto, EntryPoint = "lgLcdDeInit", ExactSpelling = true)]
		private static extern int LgLcdDeInit64();

		internal static void LgLcdDeInit() {
			if (IntPtr.Size == 8)
				LgLcdDeInit64();
			else
				LgLcdDeInit32();
		}

		#endregion


		#region Connection

		internal delegate int LgLcdOnConfigureCallback(int connection, IntPtr context);
		internal delegate int LgLcdOnNotificationCallback(int connection, IntPtr context, NotificationCode notificationCode, int notifyParam1, int notifyParam2, int notifyParam3, int notifyParam4);

		internal enum NotificationCode {
			DeviceArrival = 1,
			DeviceRemoval = 2,
			CloseConnection = 3,
			AppletDisabled = 4,
			AppletEnabled = 5,
			TerminateApplet = 6
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 8)]
		internal struct LgLcdConnectContextEx {
			internal string AppFriendlyName;
			[MarshalAs(UnmanagedType.Bool)]
			internal bool IsPersistent;
			[MarshalAs(UnmanagedType.Bool)]
			internal bool IsAutoStartable;
			internal LgLcdOnConfigureCallback ConfigCallback;
			internal IntPtr ConfigContext;
			internal int Connection;
			internal LcdAppletCapabilities AppletCapabilitiesSupported;
			internal int Reserved;
			internal LgLcdOnNotificationCallback NotificationCallback;
			internal IntPtr NotificationContext;
		}

		[SuppressUnmanagedCodeSecurity]
		[DllImport("GammaJul.LgLcd.Native32.dll", CharSet = CharSet.Auto, EntryPoint = "lgLcdConnectEx", ExactSpelling = false)]
		private static extern int LgLcdConnectEx32(ref LgLcdConnectContextEx context);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("GammaJul.LgLcd.Native64.dll", CharSet = CharSet.Auto, EntryPoint = "lgLcdConnectEx", ExactSpelling = false)]
		private static extern int LgLcdConnectEx64(ref LgLcdConnectContextEx context);

		internal static int LgLcdConnectEx(LgLcdConnectContextEx context) {
			int result = IntPtr.Size == 8
				? LgLcdConnectEx64(ref context)
				: LgLcdConnectEx32(ref context);
			if (result != ErrorSuccess)
				throw new Win32Exception(result);
			return context.Connection;
		}


		[SuppressUnmanagedCodeSecurity]
		[DllImport("GammaJul.LgLcd.Native32.dll", CharSet = CharSet.Auto, EntryPoint = "lgLcdDisconnect", ExactSpelling = true)]
		private static extern int LgLcdDisconnect32(int connection);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("GammaJul.LgLcd.Native64.dll", CharSet = CharSet.Auto, EntryPoint = "lgLcdDisconnect", ExactSpelling = true)]
		private static extern int LgLcdDisconnect64(int connection);

		internal static void LgLcdDisconnect(int connection) {
			int result = IntPtr.Size == 8
				? LgLcdDisconnect64(connection)
				: LgLcdDisconnect32(connection);
			if (result != ErrorSuccess)
				throw new Win32Exception(result);
		}

		#endregion


		#region Opening

		internal delegate int LgLcdOnSoftButtonsCallback(int device, LcdSoftButtons buttons, IntPtr context);

		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		internal struct LgLcdOpenByTypeContext {
			internal int Connection;
			internal LcdDeviceType LcdDeviceType;
			internal LgLcdOnSoftButtonsCallback SoftButtonsChangedCallback;
			internal IntPtr SoftButtonsChangedContext;
			internal int Device;
		}

		[SuppressUnmanagedCodeSecurity]
		[DllImport("GammaJul.LgLcd.Native32.dll", CharSet = CharSet.Auto, EntryPoint = "lgLcdOpenByType", ExactSpelling = true)]
		private static extern int LgLcdOpenByType32(ref LgLcdOpenByTypeContext context);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("GammaJul.LgLcd.Native64.dll", CharSet = CharSet.Auto, EntryPoint = "lgLcdOpenByType", ExactSpelling = true)]
		private static extern int LgLcdOpenByType64(ref LgLcdOpenByTypeContext context);

		internal static int LgLcdOpenByType(LgLcdOpenByTypeContext context) {
			int result = IntPtr.Size == 8
				? LgLcdOpenByType64(ref context)
				: LgLcdOpenByType32(ref context);
			if (result != ErrorSuccess)
				throw new Win32Exception(result);
			return context.Device;
		}


		[SuppressUnmanagedCodeSecurity]
		[DllImport("GammaJul.LgLcd.Native32.dll", CharSet = CharSet.Auto, EntryPoint = "lgLcdClose", ExactSpelling = true)]
		private static extern int LgLcdClose32(int device);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("GammaJul.LgLcd.Native64.dll", CharSet = CharSet.Auto, EntryPoint = "lgLcdClose", ExactSpelling = true)]
		private static extern int LgLcdClose64(int device);

		internal static void LgLcdClose(int device) {
			int result = IntPtr.Size == 8
				? LgLcdClose64(device)
				: LgLcdClose32(device);
			if (result != ErrorSuccess && result != ErrorDeviceNotConnected)
				throw new Win32Exception(result);
		}

		#endregion


		#region Soft Buttons

		[SuppressUnmanagedCodeSecurity]
		[DllImport("GammaJul.LgLcd.Native32.dll", CharSet = CharSet.Auto, EntryPoint = "lgLcdReadSoftButtons", ExactSpelling = true)]
		private static extern int LgLcdReadSoftButtons32(int device, out LcdSoftButtons buttons);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("GammaJul.LgLcd.Native64.dll", CharSet = CharSet.Auto, EntryPoint = "lgLcdReadSoftButtons", ExactSpelling = true)]
		private static extern int LgLcdReadSoftButtons64(int device, out LcdSoftButtons buttons);

		internal static LcdSoftButtons LgLcdReadSoftButtons(int device) {
			LcdSoftButtons buttons;
			int result = IntPtr.Size == 8
				? LgLcdReadSoftButtons64(device, out buttons)
				: LgLcdReadSoftButtons32(device, out buttons);
			if (result != ErrorSuccess)
				throw new Win32Exception(result);
			return buttons;
		}

		#endregion


		#region Updating Bitmap

		internal const int BmpMonoWidth = 160;
		internal const int BmpMonoHeight = 43;
		internal const int BmpMonoBpp = 8;
		internal const int BmpQvgaWidth = 320;
		internal const int BmpQvgaHeight = 240;
		internal const int BmpQvgaBpp = 32;

		private enum LgLcdBitmapFormat {
			Monochrome = 1,
			Qvga = 3
		}

		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		private struct LgLcdBitmapHeader {
			internal LgLcdBitmapFormat Format;
			internal LgLcdBitmapHeader(LgLcdBitmapFormat format) {
				Format = format;
			}
		}

		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		private class LgLcdBitmapMonochrome {
			internal LgLcdBitmapHeader Header;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = BmpMonoWidth * BmpMonoHeight * BmpMonoBpp / 8)]
			internal byte[] Pixels;
			internal LgLcdBitmapMonochrome(byte[] pixels) {
				Header = new LgLcdBitmapHeader(LgLcdBitmapFormat.Monochrome);
				Pixels = pixels;
			}
		}

		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		private class LgLcdBitmapQvga {
			internal LgLcdBitmapHeader Header;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = BmpQvgaWidth * BmpQvgaHeight * BmpQvgaBpp / 8)]
			internal byte[] Pixels;
			internal LgLcdBitmapQvga(byte[] pixels) {
				Header = new LgLcdBitmapHeader(LgLcdBitmapFormat.Qvga);
				Pixels = pixels;
			}
		}

		[SuppressUnmanagedCodeSecurity]
		[DllImport("GammaJul.LgLcd.Native32.dll", CharSet = CharSet.Auto, EntryPoint = "lgLcdUpdateBitmap", ExactSpelling = true)]
		private static extern int LgLcdUpdateBitmap32(int device, LgLcdBitmapMonochrome bitmap, int priorityAndUpdate);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("GammaJul.LgLcd.Native32.dll", CharSet = CharSet.Auto, EntryPoint = "lgLcdUpdateBitmap", ExactSpelling = true)]
		private static extern int LgLcdUpdateBitmap32(int device, LgLcdBitmapQvga bitmap, int priorityAndUpdate);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("GammaJul.LgLcd.Native64.dll", CharSet = CharSet.Auto, EntryPoint = "lgLcdUpdateBitmap", ExactSpelling = true)]
		private static extern int LgLcdUpdateBitmap64(int device, LgLcdBitmapMonochrome bitmap, int priorityAndUpdate);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("GammaJul.LgLcd.Native64.dll", CharSet = CharSet.Auto, EntryPoint = "lgLcdUpdateBitmap", ExactSpelling = true)]
		private static extern int LgLcdUpdateBitmap64(int device, LgLcdBitmapQvga bitmap, int priorityAndUpdate);

		internal static bool LgLcdUpdateBitmapMonochrome(int device, byte[] pixels, LcdPriority priority, LcdUpdateMode updateMode) {
			Debug.Assert(pixels != null && pixels.Length == BmpMonoWidth * BmpMonoHeight * BmpMonoBpp / 8);
			LgLcdBitmapMonochrome bitmap = new LgLcdBitmapMonochrome(pixels);
			int priorityAndUpdate = (int) priority | (int) updateMode;
			int result = IntPtr.Size == 8
				? LgLcdUpdateBitmap64(device, bitmap, priorityAndUpdate)
				: LgLcdUpdateBitmap32(device, bitmap, priorityAndUpdate);
			if (result != ErrorSuccess) {
				if (updateMode == LcdUpdateMode.SyncCompleteWithinFrame && result == ErrorAccessDenied)
					return false;
				throw new Win32Exception(result);
			}
			return true;
		}

		internal static bool LgLcdUpdateBitmapQvga(int device, byte[] pixels, LcdPriority priority, LcdUpdateMode updateMode) {
			Debug.Assert(pixels != null && pixels.Length == BmpQvgaWidth * BmpQvgaHeight * BmpQvgaBpp / 8);
			LgLcdBitmapQvga bitmap = new LgLcdBitmapQvga(pixels);
			int priorityAndUpdate = (int) priority | (int) updateMode;
			int result = IntPtr.Size == 8
				? LgLcdUpdateBitmap64(device, bitmap, priorityAndUpdate)
				: LgLcdUpdateBitmap32(device, bitmap, priorityAndUpdate);
			if (result != ErrorSuccess) {
				if (updateMode == LcdUpdateMode.SyncCompleteWithinFrame && result == ErrorAccessDenied)
					return false;
				throw new Win32Exception(result);
			}
			return true;
		}

		#endregion


		#region Foreground

		private enum ForegroundAppFlag {
			No = 0,
			Yes = 1
		}

		[SuppressUnmanagedCodeSecurity]
		[DllImport("GammaJul.LgLcd.Native32.dll", CharSet = CharSet.Auto, EntryPoint = "lgLcdSetAsLCDForegroundApp", ExactSpelling = true)]
		private static extern int LgLcdSetAsLcdForegroundApp32(int device, ForegroundAppFlag foregroundFlag);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("GammaJul.LgLcd.Native64.dll", CharSet = CharSet.Auto, EntryPoint = "lgLcdSetAsLCDForegroundApp", ExactSpelling = true)]
		private static extern int LgLcdSetAsLcdForegroundApp64(int device, ForegroundAppFlag foregroundFlag);

		internal static void LgLcdSetAsLcdForegroundApp(int device, bool foreground) {
			ForegroundAppFlag foregroundFlag = foreground ? ForegroundAppFlag.Yes : ForegroundAppFlag.No;
			int result = IntPtr.Size == 8
				? LgLcdSetAsLcdForegroundApp64(device, foregroundFlag)
				: LgLcdSetAsLcdForegroundApp32(device, foregroundFlag);
			if (result != ErrorSuccess)
				throw new Win32Exception(result);
		}

		#endregion


	}

}