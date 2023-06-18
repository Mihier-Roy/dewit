using Dewit.Core.Enums;

namespace Dewit.Core.Interfaces
{
	public interface IDataConverter<T>
	{
		void ToFormat(DataFormats type, string path, IEnumerable<T> items);
		IEnumerable<T> FromFormat(DataFormats type, string path);
	}
}