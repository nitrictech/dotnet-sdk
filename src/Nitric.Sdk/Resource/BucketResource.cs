using System.Collections.Generic;
using System.Linq;
using Nitric.Proto.Resource.v1;
using Nitric.Sdk.Storage;
using Action = Nitric.Proto.Resource.v1.Action;
using NitricResource = Nitric.Proto.Resource.v1.Resource;

namespace Nitric.Sdk.Resource
{
    ///<Summary>
    /// Available permissions for bucket resources.
    ///</Summary>
    public enum BucketPermission
    {
        /// <summary>
        /// Enables listing and reading files in the bucket.
        /// </summary>
        Reading,
        /// <summary>
        /// Enables adding or updating files in the bucket.
        /// </summary>
        Writing,
        /// <summary>
        /// Enables deleting files from the bucket.
        /// </summary>
        Deleting
    }

    public class BucketResource : SecureResource<BucketPermission>
    {
        internal BucketResource(string name) : base(name)
        {
        }

        public override BaseResource Register()
        {
            var resource = new NitricResource { Name = this.name, Type = ResourceType.Bucket };
            var request = new ResourceDeclareRequest { Resource = resource };
            BaseResource.client.Declare(request);
            return this;
        }

        protected override IEnumerable<Action> PermissionsToActions(IEnumerable<BucketPermission> permissions)
        {
            var actionMap = new Dictionary<BucketPermission, List<Action>>
            {
                {
                    BucketPermission.Reading,
                    new List<Action> { Action.BucketFileList, Action.BucketFileGet }
                },
                {
                    BucketPermission.Writing,
                    new List<Action> { Action.BucketFilePut }
                },
                {
                    BucketPermission.Deleting,
                    new List<Action> { Action.BucketFileDelete }
                }
            };
            return permissions.Aggregate((IEnumerable<Action>)new List<Action>(), (acc, x) => acc.Concat(actionMap[x])).Distinct();
        }

        public Storage.Bucket With(params BucketPermission[] permissions)
        {
            this.RegisterPolicy(permissions);
            return new Storage.Storage().Bucket(this.name);
        }
    }
}
