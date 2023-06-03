namespace Tests
{
  public struct MangaInfo
  {
    public string Uri;

    public long FolderSize;

    public long FilesInFolder;

    public bool AllFilesUnique;

    public string Description;

    public string Status;

    public string Name;

    public override string ToString()
    {
      return Uri;
    }
  }
}
