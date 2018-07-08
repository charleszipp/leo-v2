using System;

namespace Leo.Core.Id
{
    public interface IdProvider
    {
        string Create(DateTime? timestamp = null);

        long Hash(string id);
    }
}