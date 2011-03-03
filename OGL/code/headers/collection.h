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

#ifndef RCOLLECTION
#define RCOLLECTION




namespace Reactor
{
	/** A Growable Array Class much like a List in C#
		@remarks
			A growable array class much like a List in C# to hold collections of node objects in factory types
	*/
	template<class T> class RArray
	{
	public:
		/*@{*/
		RArray( void ){ m_pData = NULL; m_nSize = 0; m_nMaxSize = 0; }
		RArray( const RArray<T>& a ) { for( int i=0; i < a.m_nSize; i++ ) Add( a.m_pData[i] ); }
		~RArray() { RemoveAll(); }

		const T& operator[]( int nIndex ) const { return GetAt( nIndex ); }
		T&		 operator[]( int nIndex ) { return GetAt( nIndex ); }
   
		RArray& operator=( const RArray<T>& a ) { if( this == &a ) return *this; RemoveAll(); for( int i=0; i < a.m_nSize; i++ ) Add( a.m_pData[i] ); return *this; }

		RRESULT SetSize( int nNewMaxSize );
		RRESULT Add( const T& value );
		RRESULT Insert( int nIndex, const T& value );
		RRESULT SetAt( int nIndex, const T& value );
		T&      GetAt( int nIndex ) const { assert( nIndex >= 0 && nIndex < m_nSize ); return m_pData[nIndex]; }
		int     GetSize() const { return m_nSize; }
		T*		GetData() { return m_pData; }
		bool    Contains( const T& value ){ return ( -1 != IndexOf( value ) ); }

		int     IndexOf( const T& value ) { return ( m_nSize > 0 ) ? IndexOf( value, 0, m_nSize ) : -1; }
		int     IndexOf( const T& value, int iStart ) { return IndexOf( value, iStart, m_nSize - iStart ); }
		int     IndexOf( const T& value, int nIndex, int nNumElements );

		int     LastIndexOf( const T& value ) { return ( m_nSize > 0 ) ? LastIndexOf( value, m_nSize-1, m_nSize ) : -1; }
		int     LastIndexOf( const T& value, int nIndex ) { return LastIndexOf( value, nIndex, nIndex+1 ); }
		int     LastIndexOf( const T& value, int nIndex, int nNumElements );

		RRESULT Remove( int nIndex );
		void    RemoveAll() { SetSize(0); }
		void	Reset() { m_nSize = 0; }
		/*@}*/
	protected:
		/*@{*/
		T*  m_pData;        /**< the actual array of data */
		int m_nSize;        /**<  # of elements (upperBound - 1) */
		int m_nMaxSize;     /**<  max allocated */

		RRESULT SetSizeInternal( int nNewMaxSize );  /**< This version doesn't call ctor or dtor. */
		/*@}*/
	};

		// This version doesn't call ctor or dtor.
	template<typename TYPE> RRESULT RArray <TYPE>::SetSizeInternal( int nNewMaxSize )
	{
		if( nNewMaxSize < 0 || ( nNewMaxSize > INT_MAX / sizeof( TYPE ) ) )
		{
			assert( false );
			return R_INVALIDARG;
		}

		if( nNewMaxSize == 0 )
		{
			// Shrink to 0 size & cleanup
			if( m_pData )
			{
				free( m_pData );
				m_pData = NULL;
			}

			m_nMaxSize = 0;
			m_nSize = 0;
		}
		else if( m_pData == NULL || nNewMaxSize > m_nMaxSize )
		{
			// Grow array
			int nGrowBy = ( m_nMaxSize == 0 ) ? 16 : m_nMaxSize;

			// Limit nGrowBy to keep m_nMaxSize less than INT_MAX
			if( ( RUINT )m_nMaxSize + ( RUINT )nGrowBy > ( RUINT )INT_MAX )
				nGrowBy = INT_MAX - m_nMaxSize;

			nNewMaxSize = __max( nNewMaxSize, m_nMaxSize + nGrowBy );

			// Verify that (nNewMaxSize * sizeof(TYPE)) is not greater than UINT_MAX or the realloc will overrun
			if( sizeof( TYPE ) > UINT_MAX / ( RUINT )nNewMaxSize )
				return R_INVALIDARG;

			TYPE* pDataNew = ( TYPE* )realloc( m_pData, nNewMaxSize * sizeof( TYPE ) );
			if( pDataNew == NULL )
				return R_OUTOFMEMORY;

			m_pData = pDataNew;
			m_nMaxSize = nNewMaxSize;
		}

		return R_OK;
	}


	//--------------------------------------------------------------------------------------
	template<typename TYPE> RRESULT RArray <TYPE>::SetSize( int nNewMaxSize )
	{
		int nOldSize = m_nSize;

		if( nOldSize > nNewMaxSize )
		{
			assert( m_pData );
			if( m_pData )
			{
				// Removing elements. Call dtor.

				for( int i = nNewMaxSize; i < nOldSize; ++i )
					m_pData[i].~TYPE();
			}
		}

		// Adjust buffer.  Note that there's no need to check for error
		// since if it happens, nOldSize == nNewMaxSize will be true.)
		RRESULT hr = SetSizeInternal( nNewMaxSize );

		if( nOldSize < nNewMaxSize )
		{
			assert( m_pData );
			if( m_pData )
			{
				// Adding elements. Call ctor.

				for( int i = nOldSize; i < nNewMaxSize; ++i )
					::new ( &m_pData[i] ) TYPE;
			}
		}

		return hr;
	}


	//--------------------------------------------------------------------------------------
	template<typename TYPE> RRESULT RArray <TYPE>::Add( const TYPE& value )
	{
		RRESULT hr;
		if( FAILED( hr = SetSizeInternal( m_nSize + 1 ) ) )
			return hr;

		assert( m_pData != NULL );

		// Construct the new element
		::new ( &m_pData[m_nSize] ) TYPE;

		// Assign
		m_pData[m_nSize] = value;
		++m_nSize;

		return R_OK;
	}


	//--------------------------------------------------------------------------------------
	template<typename TYPE> RRESULT RArray <TYPE>::Insert( int nIndex, const TYPE& value )
	{
		RRESULT hr;

		// Validate index
		if( nIndex < 0 ||
			nIndex > m_nSize )
		{
			assert( false );
			return R_INVALIDARG;
		}

		// Prepare the buffer
		if( FAILED( hr = SetSizeInternal( m_nSize + 1 ) ) )
			return hr;

		// Shift the array
		MoveMemory( &m_pData[nIndex + 1], &m_pData[nIndex], sizeof( TYPE ) * ( m_nSize - nIndex ) );

		// Construct the new element
		::new ( &m_pData[nIndex] ) TYPE;

		// Set the value and increase the size
		m_pData[nIndex] = value;
		++m_nSize;

		return R_OK;
	}


	//--------------------------------------------------------------------------------------
	template<typename TYPE> RRESULT RArray <TYPE>::SetAt( int nIndex, const TYPE& value )
	{
		// Validate arguments
		if( nIndex < 0 ||
			nIndex >= m_nSize )
		{
			assert( false );
			return R_INVALIDARG;
		}

		m_pData[nIndex] = value;
		return R_OK;
	}


	//--------------------------------------------------------------------------------------
	// Searches for the specified value and returns the index of the first occurrence
	// within the section of the data array that extends from iStart and contains the 
	// specified number of elements. Returns -1 if value is not found within the given 
	// section.
	//--------------------------------------------------------------------------------------
	template<typename TYPE> int RArray <TYPE>::IndexOf( const TYPE& value, int iStart, int nNumElements )
	{
		// Validate arguments
		if( iStart < 0 ||
			iStart >= m_nSize ||
			nNumElements < 0 ||
			iStart + nNumElements > m_nSize )
		{
			assert( false );
			return -1;
		}

		// Search
		for( int i = iStart; i < ( iStart + nNumElements ); i++ )
		{
			if( value == m_pData[i] )
				return i;
		}

		// Not found
		return -1;
	}


	//--------------------------------------------------------------------------------------
	// Searches for the specified value and returns the index of the last occurrence
	// within the section of the data array that contains the specified number of elements
	// and ends at iEnd. Returns -1 if value is not found within the given section.
	//--------------------------------------------------------------------------------------
	template<typename TYPE> int RArray <TYPE>::LastIndexOf( const TYPE& value, int iEnd, int nNumElements )
	{
		// Validate arguments
		if( iEnd < 0 ||
			iEnd >= m_nSize ||
			nNumElements < 0 ||
			iEnd - nNumElements < 0 )
		{
			assert( false );
			return -1;
		}

		// Search
		for( int i = iEnd; i > ( iEnd - nNumElements ); i-- )
		{
			if( value == m_pData[i] )
				return i;
		}

		// Not found
		return -1;
	}



	//--------------------------------------------------------------------------------------
	template<typename TYPE> RRESULT RArray <TYPE>::Remove( int nIndex )
	{
		if( nIndex < 0 ||
			nIndex >= m_nSize )
		{
			assert( false );
			return R_INVALIDARG;
		}

		// Destruct the element to be removed
		m_pData[nIndex].~TYPE();

		// Compact the array and decrease the size
		MoveMemory( &m_pData[nIndex], &m_pData[nIndex + 1], sizeof( TYPE ) * ( m_nSize - ( nIndex + 1 ) ) );
		--m_nSize;

		return R_OK;
	}
};
#endif