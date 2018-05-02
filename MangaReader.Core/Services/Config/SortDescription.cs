using System.Collections.Generic;
using System.ComponentModel;

namespace MangaReader.Core.Services.Config
{
  [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
  public struct SortDescription
  {
    public SortDescription(string propertyName, ListSortDirection direction)
    {
      this.PropertyName = propertyName;
      this.Direction = direction;
      this.IsSealed = true;
    }
    public ListSortDirection Direction { get; set; }
    public bool IsSealed { get; }
    public string PropertyName { get; set; }

    public override bool Equals(object obj)
    {
      if (!(obj is SortDescription))
      {
        return false;
      }

      var description = (SortDescription)obj;
      return Direction == description.Direction &&
             IsSealed == description.IsSealed &&
             PropertyName == description.PropertyName;
    }

    public override int GetHashCode()
    {
      var hashCode = 586867022;
      hashCode = hashCode * -1521134295 + base.GetHashCode();
      hashCode = hashCode * -1521134295 + Direction.GetHashCode();
      hashCode = hashCode * -1521134295 + IsSealed.GetHashCode();
      hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(PropertyName);
      return hashCode;
    }

    public static bool operator ==(SortDescription description1, SortDescription description2)
    {
      return description1.Equals(description2);
    }

    public static bool operator !=(SortDescription description1, SortDescription description2)
    {
      return !(description1 == description2);
    }
  }
}