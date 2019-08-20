//
//  TimelineActivityCell.swift
//  TogglDesktop
//
//  Created by Nghia Tran on 6/21/19.
//  Copyright © 2019 Alari. All rights reserved.
//

import Cocoa

final class TimelineActivityCell: TimelineBaseCell {

    // MARK: View

    override func viewDidLoad() {
        super.viewDidLoad()
        initTrackingArea()
    }

    // MARK: Public

    func config(for activity: TimelineActivity) {
        renderBackground(with: NSColor.blue, isSmallEntry: activity.isSmall)
    }

    @IBAction func activityOnTap(_ sender: Any) {
    }
}