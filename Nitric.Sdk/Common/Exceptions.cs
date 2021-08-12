// Copyright 2021, Nitric Technologies Pty Ltd.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
using System;
using System.Collections.Generic;
using RpcStatusCode = Grpc.Core.StatusCode;

﻿namespace Nitric.Api.Common
{
    public abstract class NitricException : Exception
    {
        public static Dictionary<RpcStatusCode, Func<string, NitricException>> Exceptions = new Dictionary<RpcStatusCode, Func<string, NitricException>>
        {
            { RpcStatusCode.Cancelled, (message) => new CancelledException(message) },
            { RpcStatusCode.Unknown, (message) => new UnknownException(message) },
            { RpcStatusCode.InvalidArgument, (message) => new InvalidArgumentException(message) },
            { RpcStatusCode.DeadlineExceeded, (message) => new DeadlineExceededException(message) },
            { RpcStatusCode.NotFound, (message) => new NotFoundException(message) },
            { RpcStatusCode.AlreadyExists, (message) => new AlreadyExistsException(message) },
            { RpcStatusCode.PermissionDenied, (message) => new PermissionDeniedException(message) },
            { RpcStatusCode.ResourceExhausted, (message) => new ResourceExhaustedException(message) },
            { RpcStatusCode.FailedPrecondition, (message) => new FailedPreconditionException(message) },
            { RpcStatusCode.Aborted, (message) => new AbortedException(message) },
            { RpcStatusCode.OutOfRange, (message) => new OutOfRangeException(message) },
            { RpcStatusCode.Unimplemented, (message) => new UnimplementedException(message) },
            { RpcStatusCode.Internal, (message) => new InternalException(message) },
            { RpcStatusCode.Unavailable, (message) => new UnavailableException(message) },
            { RpcStatusCode.DataLoss, (message) => new DataLossException(message) },
            { RpcStatusCode.Unauthenticated,(message) => new UnauthenticatedException(message) },
        };
        public NitricException(string message) : base(message)
        {
        }

    }
    public class CancelledException : NitricException
    {
        public CancelledException(string message) : base(message)
        {
        }
    }
    public class UnknownException : NitricException
    {
        public UnknownException(string message) : base(message)
        {
        }
    }
    public class InvalidArgumentException : NitricException
    {
        public InvalidArgumentException(string message) : base(message)
        {
        }
    }
    public class DeadlineExceededException : NitricException
    {
        public DeadlineExceededException(string message) : base(message)
        {
        }
    }
    public class NotFoundException : NitricException
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }
    public class AlreadyExistsException : NitricException
    {
        public AlreadyExistsException(string message) : base(message)
        {
        }
    }
    public class PermissionDeniedException : NitricException
    {
        public PermissionDeniedException(string message) : base(message)
        {
        }
    }
    public class ResourceExhaustedException : NitricException
    {
        public ResourceExhaustedException(string message) : base(message)
        {
        }
    }
    public class FailedPreconditionException : NitricException
    {
        public FailedPreconditionException(string message) : base(message)
        {
        }
    }
    public class AbortedException : NitricException
    {
        public AbortedException(string message) : base(message)
        {
        }
    }
    public class OutOfRangeException : NitricException
    {
        public OutOfRangeException(string message) : base(message)
        {
        }
    }
    public class UnimplementedException : NitricException
    {
        public UnimplementedException(string message) : base(message)
        {
        }
    }
    public class InternalException : NitricException
    {
        public InternalException(string message) : base(message)
        {
        }
    }
    public class UnavailableException : NitricException
    {
        public UnavailableException(string message) : base(message)
        {
        }
    }
    public class DataLossException : NitricException
    {
        public DataLossException(string message) : base(message)
        {
        }
    }
    public class UnauthenticatedException : NitricException
    {
        public UnauthenticatedException(string message) : base(message)
        {
        }
    }
}