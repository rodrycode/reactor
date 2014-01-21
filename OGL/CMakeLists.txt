cmake_minimum_required (VERSION 2.8.9)
project (Reactor3d)

set(CMAKE_CXX_FLAGS "-std=c++11 -stdlib=libc++")

if (APPLE)
	
	set(XCODE_ATTRIBUTE_SDKROOT macosx10.7)
	set(CMAKE_OSX_DEPLOYMENT_TARGET 10.7)
	include_directories ( /System/Library/Frameworks )
	find_library(COCOA_LIBRARY Cocoa)
	find_library(OPENGL_LIBRARY OpenGL)
	find_library(GLUT_LIBRARY GLUT)
	
	set(EXTRA_LIBS ${COCOA_LIBRARY} ${GLUT_LIBRARY} ${OPENGL_LIBRARY})
	add_library (ReactorObjects OBJECT  ../code/src/RCamera.cpp
										../code/src/REngine.cpp
 										../code/src/RGame.cpp
 										../code/src/RInput.cpp)

	add_library (sReactor3d STATIC $<TARGET_OBJECTS:ReactorObjects> ${COCOA_LIBRARY} ${OPENGL_LIBRARY} ${GLUT_LIBRARY})
	add_library (Reactor3d SHARED $<TARGET_OBJECTS:ReactorObjects> ${COCOA_LIBRARY} ${OPENGL_LIBRARY} ${GLUT_LIBRARY})
	set_target_properties(sReactor3d PROPERTIES PREFIX "")
	set_target_properties(Reactor3d PROPERTIES PREFIX "")

endif (APPLE)




target_link_libraries(sReactor3d ${EXTRA_LIBS})
target_link_libraries(Reactor3d ${EXTRA_LIBS})