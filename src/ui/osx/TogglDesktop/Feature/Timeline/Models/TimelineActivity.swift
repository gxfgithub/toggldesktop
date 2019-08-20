//
//  TimelineActivity.swift
//  TogglDesktop
//
//  Created by Nghia Tran on 6/21/19.
//  Copyright © 2019 Alari. All rights reserved.
//

import Foundation

final class TimelineEvent {

    // MARK: Variables

    let title: String
    let fileName: String
    let duration: TimeInterval
    let isHeader: Bool

    // MARK: Variables

    init(title: String, fileName: String, duration: TimeInterval, isHeader: Bool) {
        self.title = title
        self.fileName = fileName
        self.duration = duration
        self.isHeader = isHeader
    }
}

final class TimelineActivity {

    // MARK: Variables

    static let DefaultDuration: TimeInterval = 60 * 15 // 15 mins
    let started: TimeInterval
    let startTimeStr: String
    let events: [TimelineEvent]
    var isSmall = false

    // MARK: Init

    init(started: TimeInterval, startTimeStr: String, events: [TimelineEvent]) {
        self.started = started
        self.startTimeStr = startTimeStr
        self.events = events
    }

    // MARK: Public

    func timechunk() -> TimeChunk? {
        return TimeChunk(start: started,
                         end: started + TimelineActivity.DefaultDuration)
    }
}