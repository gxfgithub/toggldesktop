//
//  NSCustomTimerComboBox.m
//  TogglDesktop
//
//  Created by Indrek Vändrik on 02/06/14.
//  Copyright (c) 2014 Alari. All rights reserved.
//

#import "NSCustomTimerComboBox.h"
#import "UIEvents.h"
#import "TogglDesktop-Swift.h"

@implementation NSCustomTimerComboBox

- (void)mouseDown:(NSEvent *)theEvent
{
	if (self.isEditable)
	{
		[[NSNotificationCenter defaultCenter] postNotificationOnMainThread:kForceCloseEditPopover
																	object:nil];
		return;
	}
}

- (void)drawRect:(NSRect)dirtyRect
{
	if (([[self window] firstResponder] == [self currentEditor]) && [NSApp isActive])
	{
		NSPoint origin = { 0.0, 0.0 };
		NSRect rect;
		rect.origin = origin;
		rect.size.width = [self bounds].size.width - 20;
		rect.size.height = [self bounds].size.height;

		NSBezierPath *path;
		path = [NSBezierPath bezierPathWithRect:rect];

		[NSGraphicsContext saveGraphicsState];
		NSSetFocusRingStyle(NSFocusRingOnly);
		[path fill];
		[NSGraphicsContext restoreGraphicsState];
	}
	else
	{
		[super drawRect:dirtyRect];
	}
}

@end
