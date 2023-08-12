// GetPlatformGroupMessages
// Get the messages for a group(lmc_messages > lmc_users)
use('lmc_message_db')

const message_types = ['text', 'sticker', 'image', 'video', 'audio']
const request = {
    platform_id: '',
    group_id: 'C46a278c1ae474a2e02af3929943e4582',
    group_user_id: 'Ube388d9e87bf06a6bde98435b1b286a3',
    message_types,
    limit: 10,
    sort_by: 'created_date-asc',
    start_date: '',
    text: ''
}

request.message_types = ['text']
// request.start_date = '2023-08-19';
// request.text = 'ass';

const sortField = request.sort_by.split('-')[0]
const sortType = request.sort_by.split('-')[1]
const sortNum = {
    desc: -1,
    asc: 1
}

const mo_type_format = [
    {
        $eq: [
            { $first: "$message_object.events.message.type" },
            "sticker"
        ]
    },
    [
        {
            type: "sticker",
            id: { $first: "$message_object.events.message.id" },
            stickerId: { $first: "$message_object.events.message.stickerId" },
            packageId: { $first: "$message_object.events.message.packageId" },
            stickerResourceType: { $first: "$message_object.events.message.stickerResourceType" },
        }
    ],
    "$message_object.events.message"
]

var match_stage = {
    group_id: request.group_id,
    // group_user_id: request.group_user_id,
    "message_object.events.message.type": {
        $in: request.message_types
    },
}

if (request.text.length > 0) {
    match_stage["message_object.events.message.type"] = 'text';
    match_stage["message_object.events.message.text"] = { $regex: new RegExp(request.text, "i") }
}
if (request.start_date.length > 0) {
    match_stage["created_date"] = { $regex: new RegExp(request.start_date, "i") };
}

db.lmc_messages.aggregate([
    {
        $sort: {
            [sortField]: sortNum[sortType]
        }
    },
    {
        $match: match_stage
    },
    { $limit: request.limit },
    {
        $lookup: {
            from: "lmc_users",
            localField: "group_user_id",
            foreignField: "group_user_id",
            as: "users"
        }
    },
    {
        $addFields: {
            group_user_id: { $first: "$users.group_user_id" },
            display_name: { $first: "$users.display_name" }
        }
    },
    {
        $project: {
            _id: 0,
            message_id: "$_id",
            group_id: 1,
            user: {
                group_user_id: "$group_user_id",
                display_name: "$display_name",
            },
            created_date: 1,
            modified_date: 1,
            message_object: {
                $cond: mo_type_format
            }
        }
    }
])
// .explain("executionStats")