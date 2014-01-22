cmake_minimum_required (VERSION 2.8.9)
project (Reactor3d)

set(CMAKE_CXX_FLAGS "-std=c++11 -stdlib=libc++")


set(VERSION_MAJOR 0)
set(VERSION_MINOR 1)
set(VERSION_PATCH 0)




if (APPLE)
	
	set(XCODE_ATTRIBUTE_SDKROOT macosx10.7)
	set(CMAKE_OSX_DEPLOYMENT_TARGET 10.7)
	include_directories ( /System/Library/Frameworks )
	find_library(COCOA_LIBRARY Cocoa)
	find_library(OPENGL_LIBRARY OpenGL)
	find_library(GLUT_LIBRARY GLUT)
	
	set(EXTRA_LIBS ${COCOA_LIBRARY} ${GLUT_LIBRARY} ${OPENGL_LIBRARY})
	
	file(GLOB R3D_HEADERS RELATIVE ${PROJECT_SOURCE_DIR} "/code/headers/*")
	
	add_library (ReactorObjects OBJECT  code/src/RCamera.cpp
										code/src/REngine.cpp
 										code/src/RGame.cpp
 										code/src/RInput.cpp
										code/src/RNode.cpp)

	add_library (sReactor3d STATIC $<TARGET_OBJECTS:ReactorObjects> ${EXTRA_LIBS})
	add_library (Reactor3d SHARED $<TARGET_OBJECTS:ReactorObjects> ${EXTRA_LIBS})
	set_target_properties(sReactor3d PROPERTIES PREFIX "")
	
	target_link_libraries(sReactor3d ${EXTRA_LIBS})
	target_link_libraries(Reactor3d ${EXTRA_LIBS})
	
	set_target_properties(Reactor3d PROPERTIES 
                          FRAMEWORK TRUE
                          FRAMEWORK_VERSION ${VERSION_MAJOR}.${VERSION_MINOR}.${VERSION_PATCH}
                          MACOSX_FRAMEWORK_IDENTIFIER com.reisergames.Reactor3d
                          MACOSX_FRAMEWORK_SHORT_VERSION_STRING ${VERSION_MAJOR}.${VERSION_MINOR}.${VERSION_PATCH}
                          MACOSX_FRAMEWORK_BUNDLE_VERSION ${VERSION_MAJOR}.${VERSION_MINOR}.${VERSION_PATCH}
                          PUBLIC_HEADER "${R3D_HEADERS}")
	
	
	
	add_custom_command(TARGET Reactor3d 
                       POST_BUILD
					   COMMAND mkdir -p Reactor3d.framework/Versions/${VERSION_MAJOR}.${VERSION_MINOR}.${VERSION_PATCH}/Headers
                       COMMAND cp -r ${PROJECT_SOURCE_DIR}/code/headers/* Reactor3d.framework/Versions/${VERSION_MAJOR}.${VERSION_MINOR}.${VERSION_PATCH}/Headers
					   )


endif (APPLE)





