using System;
using System.Security;
using System.Security.Permissions;

namespace GammaJul.LgLcd {

	/// <summary>
	/// Represents a code access permission used to display information on a Logitech LCD screen.
	/// </summary>
	[Serializable]
	public sealed class LgLcdPermission : CodeAccessPermission, IUnrestrictedPermission {
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
		/// Returns a value indicating whether unrestricted access to the resource
		/// protected by the permission is allowed.
		/// </summary>
		/// <returns><c>true</c> if unrestricted use of the resource protected by
		/// the permission is allowed; otherwise, <c>false</c>.</returns>
		public bool IsUnrestricted() {
			return _monochrome && _qvga;
		}

		/// <summary>
		/// Ceates an XML encoding of the security object and its current state.
		/// </summary>
		/// <returns>An XML encoding of the security object, including any state information.</returns>
		public override SecurityElement ToXml() {
			SecurityElement element = new SecurityElement("IPermission");
			element.AddAttribute("class", typeof(LgLcdPermission).AssemblyQualifiedName);
			element.AddAttribute("version", "1");
			element.AddAttribute("Monochrome", _monochrome.ToString());
			element.AddAttribute("Qvga", _qvga.ToString());
			return element;
		}

		/// <summary>
		/// Reconstructs a security object with a specified state from an XML encoding.
		/// </summary>
		/// <param name="elem">The XML encoding to use to reconstruct the security object.</param>
		/// <exception cref="ArgumentNullException">The <paramref name="elem"/> parameter is <c>null</c>.</exception>
		/// <exception cref="ArgumentException">The <paramref name="elem"/> parameter does not contain the XML
		/// encoding for an instance of the same type as the current instance.</exception>
		public override void FromXml(SecurityElement elem) {
			if (elem == null)
				throw new ArgumentNullException("elem");
			if (elem.Tag != "IPermission")
				throw new ArgumentException("The security element must be an IPermission.", "elem");
			string monochrome = elem.Attribute("Monochrome");
			try {
				_monochrome = monochrome != null && Convert.ToBoolean(monochrome);
			}
			catch (FormatException) {
				_monochrome = false;
			}
			string qvga = elem.Attribute("Qvga");
			try {
				_qvga = qvga != null && Convert.ToBoolean(qvga);
			}
			catch (FormatException) {
				_qvga = false;
			}
		}

		/// <summary>
		/// Creates and returns an identical copy of the current permission object.
		/// </summary>
		/// <returns>A copy of the current permission object.</returns>
		public override IPermission Copy() {
			return new LgLcdPermission(_monochrome, _qvga);
		}

		/// <summary>
		/// Creates and returns a permission that is the intersection of the current permission and the specified permission.
		/// </summary>
		/// <returns>
		/// A new permission that represents the intersection of the current permission and the specified permission.
		/// This new permission is <c>null</c> if the intersection is empty.
		/// </returns>
		/// <param name="target">A permission to intersect with the current permission.
		/// It must be of the same type as the current permission.</param>
		/// <exception cref="ArgumentException">The <paramref name="target"/> parameter is not <c>null</c>
		/// and is not an instance of the same class as the current permission.</exception>
		public override IPermission Intersect(IPermission target) {
			if (target == null)
				return null;
			LgLcdPermission permission = target as LgLcdPermission;
			if (permission == null)
				throw new ArgumentException("The target permission must be of type LgLcdPermission.", "target");
			bool monochrome = _monochrome && permission._monochrome;
			bool qvga = _qvga && permission._qvga;
			return monochrome || qvga ? new LgLcdPermission(monochrome, qvga) : null;
		}

		/// <summary>
		/// Determines whether the current permission is a subset of the specified permission.
		/// </summary>
		/// <returns><c>true</c> if the current permission is a subset of the specified permission;
		/// otherwise, <c>false</c>.</returns>
		/// <param name="target">A permission that is to be tested for the subset relationship.
		/// This permission must be of the same type as the current permission.</param>
		/// <exception cref="ArgumentException">The <paramref name="target"/> parameter is
		/// not <c>null</c> and is not of the same type as the current permission.</exception>
		public override bool IsSubsetOf(IPermission target) {
			if (target == null)
				return !_monochrome && !_qvga;
			LgLcdPermission permission = target as LgLcdPermission;
			if (permission == null)
				throw new ArgumentException("The target permission must be of type LgLcdPermission.", "target");
			if (permission.IsUnrestricted())
				return true;
			if (IsUnrestricted())
				return false;
			return _monochrome == permission._monochrome && _qvga == permission._qvga;
		}

		/// <summary>
		/// Creates a new <see cref="LgLcdPermission"/> with the specified
		/// monochrome and QVGA states.
		/// </summary>
		/// <param name="monochrome">Whether the use of monochrome devices is allowed.</param>
		/// <param name="qvga">Whether the use of QVGA devices is allowed.</param>
		public LgLcdPermission(bool monochrome, bool qvga) {
			_monochrome = monochrome;
			_qvga = qvga;
		}
        
		/// <summary>
		/// Creates a bew <see cref="LgLcdPermission"/> with the specified state.
		/// </summary>
		/// <param name="state">State determining the permission.</param>
		public LgLcdPermission(PermissionState state) {
			_monochrome = _qvga = state == PermissionState.Unrestricted;
		}
	}

}