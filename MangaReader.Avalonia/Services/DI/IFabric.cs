using System.Threading.Tasks;

namespace MangaReader.Avalonia.Services
{

  public interface ITaskFabric<in TInput, TOutput>
  {
    Task<TOutput> Create(TInput input);
  }

  public interface IFabric<in TInput, out TOutput>
  {
    TOutput Create(TInput input);
  }
 
}
