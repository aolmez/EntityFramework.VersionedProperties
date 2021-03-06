﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;

#if EF_CORE
namespace EntityFrameworkCore.VersionedProperties {
#else
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
namespace EntityFramework.VersionedProperties {
#endif

#if DEBUG
	public
#else
	internal
#endif
	interface IVersion {}

	[DebuggerDisplay("Value = {Value}")]
	public abstract class VersionBase<TValue> : IVersion, IEquatable<VersionBase<TValue>> {
		/// <summary>Gets the unique identifier for this version of the data</summary>
		public Int64 Id { get; internal set; }

		/// <summary>Gets the identifier for the entity which this verion represents</summary>
#if !EF_CORE
		// TODO: check if EF Core has IndexAttribute yet
		[Index]
#endif
		public Guid VersionedId { get; internal set; }

		/// <summary>Gets the date-time representing when this version was first assigned</summary>
		public DateTime Added { get; internal set; }

		/// <summary>Gets the value of this verison of the data</summary>
		public TValue Value { get; internal set; }

		public override String ToString() => Value?.ToString() ?? String.Empty;

		#region Equality
		public override Boolean Equals(Object obj) => Equals(obj as VersionBase<TValue>);

		public Boolean Equals(VersionBase<TValue> version) {
			if (ReferenceEquals(version, null))
				return false;
			if (ReferenceEquals(version, this))
				return true;
			return Id == version.Id
			    && VersionedId == version.VersionedId
			    && Added == version.Added
			    && ValueEqualityComparer.Equals(Value, version.Value);
		}

		public override Int32 GetHashCode() {
			var hash = 1374496523;
			unchecked {
				hash = (hash * -1521134295) + Id.GetHashCode();
				hash = (hash * -1521134295) + VersionedId.GetHashCode();
				hash = (hash * -1521134295) + Added.GetHashCode();
				if (IsTValueAValueType || !ReferenceEquals(Value, null))
					hash = (hash * -1521134295) + ValueEqualityComparer.GetHashCode(Value);
			}
			return hash;
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly Boolean IsTValueAValueType = typeof(TValue).GetTypeInfo().IsValueType;

		protected virtual IEqualityComparer<TValue> ValueEqualityComparer => EqualityComparer<TValue>.Default;
		#endregion
	}

	public abstract class RequiredValueVersionBase<TValue> : VersionBase<TValue>
	where TValue : class {
		[Required]
		public new TValue Value {
			get { return base.Value; }
			internal set { base.Value = value; }
		}
	}

	#region Primitives
	public sealed class BooleanVersion                : VersionBase<Boolean       > {}
	public sealed class DateTimeVersion               : VersionBase<DateTime      > {}
	public sealed class DateTimeOffsetVersion         : VersionBase<DateTimeOffset> {}
	public sealed class DecimalVersion                : VersionBase<Decimal       > {}
	public sealed class DoubleVersion                 : VersionBase<Double        > {}
	public sealed class SingleVersion                 : VersionBase<Single        > {}
	public sealed class GuidVersion                   : VersionBase<Guid          > {}
	public sealed class Int16Version                  : VersionBase<Int16         > {}
	public sealed class Int32Version                  : VersionBase<Int32         > {}
	public sealed class Int64Version                  : VersionBase<Int64         > {}
	public sealed class ByteVersion                   : VersionBase<Byte          > {}
	#endregion
	#region Nullable primitives
	public sealed class NullableBooleanVersion        : VersionBase<Boolean?       > { }
	public sealed class NullableDateTimeVersion       : VersionBase<DateTime?      > { }
	public sealed class NullableDateTimeOffsetVersion : VersionBase<DateTimeOffset?> { }
	public sealed class NullableDecimalVersion        : VersionBase<Decimal?       > { }
	public sealed class NullableDoubleVersion         : VersionBase<Double?        > { }
	public sealed class NullableSingleVersion         : VersionBase<Single?        > { }
	public sealed class NullableGuidVersion           : VersionBase<Guid?          > { }
	public sealed class NullableInt16Version          : VersionBase<Int16?         > { }
	public sealed class NullableInt32Version          : VersionBase<Int32?         > { }
	public sealed class NullableInt64Version          : VersionBase<Int64?         > { }
	public sealed class NullableByteVersion           : VersionBase<Byte?          > { }
	#endregion
	public sealed class StringVersion                 : VersionBase<String> {}
	public sealed class RequiredStringVersion         : RequiredValueVersionBase<String> {}
	public sealed class ByteArrayVersion              : VersionBase<Byte[]> {}
	public sealed class RequiredByteArrayVersion      : RequiredValueVersionBase<Byte[]> {}
#if !EF_CORE
	public sealed class DbGeographyVersion            : VersionBase<DbGeography> {}
	public sealed class DbGeometryVersion             : VersionBase<DbGeometry> {}
	public sealed class RequiredDbGeographyVersion    : RequiredValueVersionBase<DbGeography> {}
	public sealed class RequiredDbGeometryVersion     : RequiredValueVersionBase<DbGeometry> {}
#endif

}