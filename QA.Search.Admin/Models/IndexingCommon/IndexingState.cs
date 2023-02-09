﻿namespace QA.Search.Admin.Models.IndexingCommon
{
    public enum IndexingState
    {
        Running = 0,
        Stopped = 1,
        AwaitingRun = 2,
        AwaitingStop = 3,
        Error = 4
    }
}
