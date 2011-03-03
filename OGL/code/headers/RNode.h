/*
Reactor 3D MIT License

Copyright (c) 2010 Reiser Games

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
#ifndef __RNODE__
#define __RNODE__

#include "common.h"
#include "collection.h"
using namespace std;
namespace Reactor {
	
	class RNode
	{
	private:
		RArray<RNode>* childNodes;
		RNode* parent;
		string name;
	public:
		RNode(){ childNodes = new RArray<RNode>(); parent = NULL;};
		~RNode(){ childNodes->RemoveAll(); delete childNodes;};
		
		string GetName(){ return name; };
		void SetName(string& Name){ name = Name;};
		void SetParent(RNode& ParentNode){ parent = &ParentNode; };
		RNode* GetParent();
		
		void AddChild(RNode* ChildNode);
		RNode* GetChild(RINT index);
		RNode* GetChild(string& Name);
		
		/*RBOOL operator == (const RNode& n)
		{
			RNode node = n;
			if(GetName() == node.GetName())
				return true;
			else {
				return false;
			}

		}*/
		
	};
};

#endif