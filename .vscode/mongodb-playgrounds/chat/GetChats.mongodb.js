use('lmc_chat_db')

const request = {
    platform_id: '64d46d7ee60d7c9d3d2940d8',
    group_id: 'C46a278c1ae474a2e02af3929943e4582',
    group_user_id: 'Ube388d9e87bf06a6bde98435b1b286a3',
    limit: 10,
    sort_by: 'created_date-asc',
    start_date: '',
}

const sortField = request.sort_by.split('-')[0]
const sortType = request.sort_by.split('-')[1]
const sortNum = {
    desc: -1,
    asc: 1
}

db.lmc_chats.aggregate([
    {
        $sort: {
          [sortField]: sortNum[sortType]
        }
    },
    {
        $match: {
          platform_id: request.platform_id
        }
    },
    {
        $limit: request.limit
    },
    {
        $project: {
          _id: 0,
          chat_id: "$_id",
          group: 1,
          created_date: 1,
          modified_date: 1,
          latest_message: 1
        }
    }
])