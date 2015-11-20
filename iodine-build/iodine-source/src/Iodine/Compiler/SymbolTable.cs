/**
  * Copyright (c) 2015, GruntTheDivine All rights reserved.

  * Redistribution and use in source and binary forms, with or without modification,
  * are permitted provided that the following conditions are met:
  * 
  *  * Redistributions of source code must retain the above copyright notice, this list
  *    of conditions and the following disclaimer.
  * 
  *  * Redistributions in binary form must reproduce the above copyright notice, this
  *    list of conditions and the following disclaimer in the documentation and/or
  *    other materials provided with the distribution.

  * Neither the name of the copyright holder nor the names of its contributors may be
  * used to endorse or promote products derived from this software without specific
  * prior written permission.
  * 
  * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY
  * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
  * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT
  * SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
  * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
  * TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR
  * BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
  * CONTRACT ,STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN
  * ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH
  * DAMAGE.
**/

using System;
using System.Collections.Generic;

namespace Iodine.Compiler
{
	/// <summary>
	/// Symbol table.
	/// </summary>
	public class SymbolTable
	{
		class LocalScope
		{
			public int NextLocal { set; get; }

			public LocalScope ParentScope { private set; get; }

			public LocalScope (LocalScope parentScope)
			{
				ParentScope = parentScope;
				NextLocal = 0;
			}

		}

		private int nextGlobalIndex = 0;
		private Scope globalScope = new Scope ();
		private Scope lastScope = null;
		private LocalScope currentLocalScope = null;

		public Scope CurrentScope { private set; get; }

		public SymbolTable ()
		{
			CurrentScope = globalScope;
		}

		public Scope NextScope ()
		{
			if (CurrentScope == null)
				CurrentScope = globalScope;
			CurrentScope = CurrentScope.NextScope;
			return CurrentScope;
		}

		/// <summary>
		/// Leaves the scope.
		/// </summary>
		/// <returns>The scope.</returns>
		public Scope LeaveScope ()
		{
			Scope old = CurrentScope;
			CurrentScope = old.ParentScope;
			CurrentScope.NextScope = old.NextScope;
			return CurrentScope;
		}

		/// <summary>
		/// Begins a new scope.
		/// </summary>
		/// <param name="isLocalScope">If set to <c>true</c> is local scope.</param>
		public void BeginScope (bool isLocalScope = false)
		{
			if (isLocalScope) {
				currentLocalScope = new LocalScope (currentLocalScope);
			}
			Scope newScope = new Scope (CurrentScope);
			if (lastScope != null) {
				lastScope.NextScope = newScope;
			} else {
				globalScope.NextScope = newScope;
			}
			CurrentScope.AddScope (newScope);
			CurrentScope = newScope;
			lastScope = newScope;
		}

		/// <summary>
		/// Ends a scope.
		/// </summary>
		/// <param name="isLocalScope">If set to <c>true</c> is local scope.</param>
		public void EndScope (bool isLocalScope = false)
		{
			if (isLocalScope) {
				currentLocalScope = currentLocalScope.ParentScope;
			}

			CurrentScope = CurrentScope.ParentScope;
		}

		/// <summary>
		/// Adds a symbol.
		/// </summary>
		/// <returns>The symbol.</returns>
		/// <param name="name">Symbol name.</param>
		public int AddSymbol (string name)
		{
			if (this.CurrentScope.ParentScope != null) {
				return CurrentScope.AddSymbol (SymbolType.Local, name, currentLocalScope.NextLocal++);
			}
			return CurrentScope.AddSymbol (SymbolType.Global, name, nextGlobalIndex++);
		}

		/// <summary>
		/// Determines whether this scope has a definition for a specified symbol name.
		/// </summary>
		/// <returns><c>true</c> if this instance name is defined; otherwise, <c>false</c>.</returns>
		/// <param name="name">Name.</param>
		public bool IsSymbolDefined (string name)
		{
			Scope curr = CurrentScope;
			while (curr != null) {
				Symbol sym;
				if (curr.GetSymbol (name, out sym)) {
					return true;
				}
				curr = curr.ParentScope;
			}
			return false;
		}

		/// <summary>
		/// Gets a symbol based on its name, returns null if the symbol is not defined
		/// </summary>
		/// <returns>A symbol.</returns>
		/// <param name="name">The symbol name.</param>
		public Symbol GetSymbol (string name)
		{
			Scope curr = CurrentScope;
			while (curr != null) {
				Symbol sym;
				if (curr.GetSymbol (name, out sym)) {
					return sym;
				}
				curr = curr.ParentScope;
			}
			return null;
		}
	}

	/// <summary>
	/// Scope.
	/// </summary>
	public class Scope
	{
		private List<Symbol> symbols = new List<Symbol> ();
		private List<Scope> childScopes = new List<Scope> ();

		public readonly Scope ParentScope;

		public Scope NextScope { set; get; }

		public IList<Scope> ChildScopes {
			get {
				return childScopes;
			}
		}

		public int SymbolCount {
			get {
				int val = symbols.Count;
				foreach (Scope scope in childScopes) {
					val += scope.SymbolCount;
				}
				return val;
			}
		}

		public Scope ()
		{
			ParentScope = null;
		}

		public Scope (Scope parent)
		{
			ParentScope = parent;
		}

		public int AddSymbol (SymbolType type, string name, int index)
		{
			symbols.Add (new Symbol (type, name, index));
			return index;
		}

		public void AddScope (Scope scope)
		{
			childScopes.Add (scope);
		}

		public bool GetSymbol (string name, out Symbol symbol)
		{
			foreach (Symbol sym in symbols) {
				if (sym.Name == name) {
					symbol = sym;
					return true;
				}
			}
			symbol = null;
			return false;
		}
	}
}

