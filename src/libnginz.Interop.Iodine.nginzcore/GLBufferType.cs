using System;
using Iodine.Runtime;
using nginz.Common;
using OpenTK.Graphics.OpenGL4;

namespace nginz.Interop.Iodine.nginzcore
{
	public class GLBufferType : IodineObject, ICanLog
	{
		readonly public static IodineTypeDefinition typeDef;

		static GLBufferType () {
			typeDef = new GLBufferTypeDefinition ("GLBuffer");
		}

		readonly object GLBufferObject;
		readonly Type GLBufferOriginalType;

		class GLBufferTypeDefinition : IodineTypeDefinition, ICanLog {
			public GLBufferTypeDefinition (string name) : base (name) { }

			enum ArrayType {
				Undefined,
				Integer,
			}

			// arguments [0]: BufferTargetType
			// arguments [1]: Buffer initialization array
			// arguments [2]: BufferUsageHint
			public override IodineObject Invoke (VirtualMachine vm, IodineObject[] arguments) {

				// Check if all arguments were passed
				if (arguments.Length < 3) {
					this.IodineError ("GLBuffer: Expected 3 arguments.");
					return null;
				}

				// Read the BufferTarget
				var targetType = arguments [0] as IodineInteger;

				// Check if the BufferTarget is null
				if (targetType == null) {
					this.IodineError ("GLBuffer: Invalid BufferTarget");
					return null;
				}

				// Read the initialization array
				bool arrayError = false;
				ArrayType arrayType = ArrayType.Undefined;
				IodineList theList = null;

				TypeSwitch.On (arguments [1])
					.Case ((IodineList list) => {

						// Check the length of the list
						var len = list.Objects.Count;

						// Check if the length is 0
						if (len == 0) {
							this.IodineError ("GLBuffer: Cannot work with an empty array");
							arrayError = true;
						}

						// Get the first element
						var first = list.Objects [0];
						TypeSwitch.On (first)
							.Case ((IodineInteger x) => arrayType = ArrayType.Integer)
							.Default (x => {
								var type = x.GetType ().Name;
								this.IodineError ("GLBuffer: Invalid array element type: '{0}'", type);
								arrayError = true;
							});

						// Set the list object to the actual list
						theList = list;
				})
					.Default (obj => {

						// Get the name of the type
						var type = obj.GetType ().Name;

						// Tell the user that the type is not supported
						this.IodineError ("GLBuffer: Not an array type: '{0}'", type);
						arrayError = true;
				});

				// Return null if there was an error
				if (arrayError)
					return null;

				// Read the BufferUsageHint
				var hintType = arguments [2] as IodineInteger;

				// Check if the BufferUsageHint is null
				if (hintType == null) {
					this.IodineError ("GLBuffer: Invalid BufferUsageHint");
					return null;
				}

				// Create the BufferTarget
				var bufferTarget = (BufferTarget) targetType.Value;

				// Create the BufferUsageHint
				var bufferUsageHint = (BufferUsageHint) hintType.Value;

				this.IodineInfo ("GLBuffer: Accepted BufferTarget '{0}'", bufferTarget);
				this.IodineInfo ("GLBuffer: Accepted BufferUsageHint '{0}'", bufferUsageHint);

				// Create the GLBuffer object
				object glBufferObject;

				// Switch on the array type
				switch (arrayType) {
				case ArrayType.Integer:
					{
						var buffer = new long[theList.Objects.Count];
						for (var i = 0; i < buffer.Length; i++)
							buffer [i] = (theList.Objects [i] as IodineInteger).Value;
						glBufferObject = new GLBuffer<long> (
							target: bufferTarget,
							hint: bufferUsageHint,
							buffer: buffer
						);
						return new GLBufferType (glBufferObject, typeof(GLBuffer<long>));
					}
				}

				return null;
			}
		}

		public GLBufferType (object glBufferObj, Type glBufferType) : base (typeDef) {
			GLBufferObject = glBufferObj;
			GLBufferOriginalType = glBufferType;
		}
	}
}

