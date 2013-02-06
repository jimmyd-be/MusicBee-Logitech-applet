using System;
using System.Security;
using System.Security.Permissions;

namespace GammaJul.LgLcd {

	/// <summary>
	/// Allows security actions for <see cref="LgLcdPermission"/> to be applied to code using declarative security.
	/// </summary>
	[Serializable]
	[AttributeUsageAttribute(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	public class LgLcdPermissionAttribute : CodeAccessSecurityAttribute {
		private bool _monochrome;
		private bool _qvga;

		/// <summary>
		/// Gets or sets whether the usage of monochrome devices is allowed.
		/// </summary>
		public bool Monochrome {
			get { return _monochrome; }
			set { _monochrome = value; }
		}

		/// <summary>
		/// Gets or sets whether the usage of QVGA devices is allowed.
		/// </summary>
		public bool Qvga {
			get { return _qvga; }
			set { _qvga = value; }
		}

		/// <summary>
		/// Creates a permission object that can then be serialized into binary form and
		/// persistently stored along with the <see cref="SecurityAction"/> in an assembly's metadata.
		/// </summary>
		/// <returns>A serializable permission object.</returns>
		public override IPermission CreatePermission() {
			return Unrestricted ? new LgLcdPermission(PermissionState.Unrestricted) : new LgLcdPermission(_monochrome, _qvga);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LgLcdPermissionAttribute"/> class
		/// with the specified <see cref="SecurityAction"/>. 
		/// </summary>
		/// <param name="action">One of the <see cref="SecurityAction"/> values.</param>
		public LgLcdPermissionAttribute(SecurityAction action)
			: base(action) {
		}
	}

}