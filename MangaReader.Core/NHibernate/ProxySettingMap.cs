using FluentNHibernate.Mapping;
using MangaReader.Core.Account;

namespace MangaReader.Core.NHibernate
{
  public class ProxySettingMap : ClassMap<ProxySetting>
  {
    public ProxySettingMap()
    {
      Not.LazyLoad();
      Id(x => x.Id);
      Map(x => x.Name);
      Map(x => x.UserName);
      Map(x => x.Password);
      Map(x => x.SettingType);
      Map(x => x.Address);
    }
  }
}
