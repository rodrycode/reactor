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

#include "../headers/RNode.h"

namespace Reactor{
	
	RNode::RNode(){
		this->childNodes = new RArray<RNode>();
	}
	RNode::~RNode(){
		delete this->childNodes;
	}
	
	string RNode::GetName(){
		return this->name;
	}
	
	void RNode::SetName(const string& Name){
		this->name = name;
	}
	
	void RNode::SetParent(const RNode& ParentNode){
		this->parent = &ParentNode;
	}
	
	const RNode& RNode::GetParent(){
		return *this->parent;
	}
	
	void RNode::AddChild(const RNode& ChildNode){
		this->childNodes->Add(ChildNode);
	}
	
	const RNode& RNode::GetChild(RINT index){
		return this->childNodes->GetAt(index);
	}
	
	const RNode* RNode::GetChild(const string& name){
		
		for(int i=0; i<this->childNodes->GetSize(); i++){
			RNode& t = this->childNodes->GetAt(i);
			if(t.GetName() == name){
				return &t;
			}
		}
		return NULL;
	}
	
	RBOOL RNode::operator == (const RNode& n){
		if(GetName() == n.name){
			return true;
		}
		return false;
	}
}