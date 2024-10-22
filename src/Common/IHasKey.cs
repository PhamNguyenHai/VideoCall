using System;

namespace PetProject
{
    public interface IHasKey
    {
        /// <summary>
        /// Để get id của entity
        /// </summary>
        /// <returns>id của entity</returns>
        Guid GetKey();

        /// <summary>
        /// Để set key của entity
        /// </summary>
        /// <param name="key">key cần set</param>
        void SetKey(Guid key);
    }
}
