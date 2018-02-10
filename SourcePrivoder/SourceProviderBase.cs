﻿using LyricDisplayerPlugin.SourcePrivoder;
using LyricDisplayerPlugin.SourcePrivoder.QQMusic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LyricDisplayerPlugin
{
    public abstract class SourceProviderBase
    {
        public abstract Lyrics ProvideLyric(string artist, string title, int time);
    }

    public abstract class SourceProviderBase<SEARCHER,DOWNLOADER,PARSER>:SourceProviderBase where DOWNLOADER:LyricDownloaderBase,new() where PARSER:LyricParserBase,new() where SEARCHER:SongSearchBase,new()
    {
        public int DurationThresholdValue { get; set; } = 1000;

        public DOWNLOADER Downloader { get; } = new DOWNLOADER();
        public SEARCHER Seadrcher { get; } = new SEARCHER();
        public PARSER Parser { get; } = new PARSER();

        public override Lyrics ProvideLyric(string artist, string title, int time)
        {
            var search_result = Seadrcher.Search(artist, title).Result;

#if DEBUG
            foreach (var r in search_result)
            {
                Utils.Debug($"- music_id:{r.ID} artist:{r.Artist} title:{r.Title} time{r.Duration * 1000}({Math.Abs(r.Duration * 1000 - time):F2})");
            }
#endif

            search_result.RemoveAll((r) => Math.Abs(r.Duration * 1000 - time) > DurationThresholdValue);

            string check_Str = $"{artist}{title}";

            search_result.Sort((a, b) => _GetEditDistance(a) - _GetEditDistance(b));

            if (search_result.Count == 0)
            {
                return null;
            }

            var result = search_result.First();

            Utils.Debug($"Picked music_id:{result.ID} artist:{result.Artist} title:{result.Title}");

            var lyric_cont = Downloader.DownloadLyric(result);

            if (string.IsNullOrWhiteSpace(lyric_cont))
            {
                return null;
            }

            return Parser.Parse(lyric_cont);

            int _GetEditDistance(SearchSongResultBase s) => Utils.EditDistance($"{s.Artist}{s.Title}", check_Str);
        }
    }
}
