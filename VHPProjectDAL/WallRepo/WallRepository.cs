//using Microsoft.EntityFrameworkCore;
//using MySqlConnector;
//using System.Data;
//using VHPProjectCommonUtility.Logger;
//using VHPProjectDAL.DataModel;
//using VHPProjectDAL.Execution;
//using VHPProjectDTOModel.WallDTO.request;
//using VHPProjectDTOModel.WallDTO.response;

//namespace VHPProjectDAL.Repository.WallRepo
//{
//    public class WallRepository : IWallRepository
//    {
//        private readonly MasterProjContext _context;
//        private readonly ILoggerManager _logger;

//        public WallRepository(MasterProjContext context, ILoggerManager logger)
//        {
//            _context = context;
//            _logger = logger;
//        }

//        // ---------------------------------------------------------
//        // INSERT COMMENT
//        // ---------------------------------------------------------
//        public async Task<int> AddCommentAsync(WallPostComments c)
//        {
//            string sql = @"
//                INSERT INTO wall_post_comments 
//                (wall_id, member_details_id, comment_text, insert_date_time, active)
//                VALUES (@wallId, @memberId, @comment, NOW(), 1);
//                SELECT LAST_INSERT_ID();";

//            var parameters = new[]
//            {
//                new MySqlParameter("@wallId", c.WallId),
//                new MySqlParameter("@memberId", c.MemberDetailsId),
//                new MySqlParameter("@comment", c.CommentText)
//            };

//            return await _context.Database.ExecuteScalarAsync(
//                sql,
//                r => r.GetInt32(0),
//                parameters
//            );
//        }

//        // ---------------------------------------------------------
//        // ADD / UPDATE LIKE
//        // ---------------------------------------------------------
//        public async Task AddOrUpdateLikeAsync(WallPostLike like)
//        {
//            string sql = @"
//                INSERT INTO wall_post_like (member_details_id, wall_id, `like`, created_date, active)
//                VALUES (@memberId, @wallId, @like, NOW(), 1)
//                ON DUPLICATE KEY UPDATE 
//                   `like` = VALUES(`like`),
//                   active = VALUES(active),
//                   created_date = NOW();
//                SELECT 1;";

//            var parameters = new[]
//            {
//                new MySqlParameter("@memberId", like.MemberDetailsId),
//                new MySqlParameter("@wallId", like.WallId),
//                new MySqlParameter("@like", like.Like)
//            };

//            await _context.Database.ExecuteScalarAsync<int>(
//                sql,
//                r => r.GetInt32(0),
//                parameters
//            );
//        }

//        // ---------------------------------------------------------
//        // INSERT WALL
//        // ---------------------------------------------------------
//        public async Task<int> AddWallAsync(Wall w)
//        {
//            string sql = @"
//                INSERT INTO wall (member_details_id, post_messages, created_date, active)
//                VALUES (@memberId, @msg, NOW(), 1);
//                SELECT LAST_INSERT_ID();";

//            var parameters = new[]
//            {
//                new MySqlParameter("@memberId", w.MemberDetailsId),
//                new MySqlParameter("@msg", w.PostMessages)
//            };

//            return await _context.Database.ExecuteScalarAsync(
//                sql,
//                r => r.GetInt32(0),
//                parameters
//            );
//        }

//        // ---------------------------------------------------------
//        // INSERT WALL IMAGES
//        // ---------------------------------------------------------
//        public async Task AddWallImagesAsync(IEnumerable<WallPostImages> images)
//        {
//            foreach (var img in images)
//            {
//                string sql = @"
//                    INSERT INTO wall_post_images (wall_id, image_url, created_date, active)
//                    VALUES (@wallId, @url, NOW(), 1);
//                    SELECT 1;";

//                var parameters = new[]
//                {
//                    new MySqlParameter("@wallId", img.WallId),
//                    new MySqlParameter("@url", img.ImageUrl)
//                };

//                await _context.Database.ExecuteScalarAsync<int>(
//                    sql,
//                    r => r.GetInt32(0),
//                    parameters
//                );
//            }
//        }

//        // ---------------------------------------------------------
//        // GET LIKE BY MEMBER + WALL
//        // ---------------------------------------------------------
//        public async Task<WallPostLike?> GetLikeByMemberAndWallAsync(int memberId, int wallId)
//        {
//            string sql = @"
//                SELECT member_details_id, wall_id, `like`, active, created_date
//                FROM wall_post_like
//                WHERE member_details_id = @memberId
//                  AND wall_id = @wallId
//                  AND active = 1
//                LIMIT 1;";

//            var parameters = new[]
//            {
//                new MySqlParameter("@memberId", memberId),
//                new MySqlParameter("@wallId", wallId)
//            };

//            return await _context.Database.ExecuteScalarAsync(
//                sql,
//                r => new WallPostLike
//                {
//                    MemberDetailsId = r.GetInt32("member_details_id"),
//                    WallId = r.GetInt32("wall_id"),
//                    Like = r.GetBoolean("like") ? (ulong?)1 : 0,
//                    Active = (ulong?)r.GetInt32("active"),
//                    CreatedDate = r.GetDateTime("created_date")
//                },
//                parameters
//            );
//        }

//        // ---------------------------------------------------------
//        // TOTAL WALL COUNT
//        // ---------------------------------------------------------
//        public async Task<int> GetTotalWallCountAsync()
//        {
//            string sql = @"SELECT COUNT(*) FROM wall WHERE active IN (0,1);";

//            return await _context.Database.ExecuteScalarAsync(
//                sql,
//                r => r.GetInt32(0)
//            );
//        }

//        public async Task<List<WallDataResponse>> GetWallDataAsync(GetWallDataRequest req, int skip, int take)
//        {
//            var result = new List<WallDataFlat>();

//            string sql = @"
//        SELECT 
//            w.id_wall AS IdWall,
//            w.member_details_id AS MemberId,
//            CONCAT(m.first_name, ' ', m.last_name) AS MemberName,
//            w.post_messages AS PostMessage,
//            w.created_date AS WallDate,

//            -- images
//            GROUP_CONCAT(DISTINCT i.image_url ORDER BY i.id_wall_post_images SEPARATOR ',') AS ImageUrls,

//            -- like count
//            (SELECT COUNT(*) FROM wall_post_like 
//             WHERE wall_id = w.id_wall AND active = 1 AND `like` = 1) AS LikeCount,

//            -- comment count
//            (SELECT COUNT(*) FROM wall_post_comments 
//             WHERE wall_id = w.id_wall AND active = 1) AS CommentCount,

//            -- comment details
//            c.id_wall_post_comments AS CommentId,
//            c.comment_text AS CommentText,
//            c.member_details_id AS CommentMemberId,
//            CONCAT(cm.first_name, ' ', cm.last_name) AS CommentMemberName,
//            c.insert_date_time AS CommentDate

//        FROM wall w
//        LEFT JOIN member_details m ON w.member_details_id = m.id_member
//        LEFT JOIN wall_post_images i ON w.id_wall = i.wall_id AND i.active = 1
//        LEFT JOIN wall_post_comments c ON w.id_wall = c.wall_id AND c.active = 1
//        LEFT JOIN member_details cm ON c.member_details_id = cm.id_member

//        WHERE w.active IN (1,2)

//        GROUP BY w.id_wall, c.id_wall_post_comments

//        ORDER BY w.created_date DESC
//        LIMIT @take OFFSET @skip;
//    ";

//            using (var conn = new MySqlConnection(connectionString))
//            {
//                await conn.OpenAsync();

//                using (var cmd = new MySqlCommand(sql, conn))
//                {
//                    cmd.Parameters.AddWithValue("@skip", skip);
//                    cmd.Parameters.AddWithValue("@take", take);

//                    using (var reader = await cmd.ExecuteReaderAsync())
//                    {
//                        while (await reader.ReadAsync())
//                        {
//                            var row = new WallDataFlat
//                            {
//                                IdWall = reader.GetInt32("IdWall"),
//                                MemberId = reader.GetInt32("MemberId"),
//                                MemberName = reader["MemberName"] as string,
//                                PostMessage = reader["PostMessage"].ToString(),
//                                WallDate = reader.GetDateTime("WallDate"),

//                                ImageUrls = reader["ImageUrls"] == DBNull.Value ? "" : reader["ImageUrls"].ToString(),

//                                LikeCount = reader.GetInt32("LikeCount"),
//                                CommentCount = reader.GetInt32("CommentCount"),

//                                CommentId = reader["CommentId"] == DBNull.Value ? null : (int?)reader.GetInt32("CommentId"),
//                                CommentText = reader["CommentText"] as string,
//                                CommentMemberId = reader["CommentMemberId"] == DBNull.Value ? null : (int?)reader.GetInt32("CommentMemberId"),
//                                CommentMemberName = reader["CommentMemberName"] as string,
//                                CommentDate = reader["CommentDate"] == DBNull.Value ? null : (DateTime?)reader.GetDateTime("CommentDate")
//                            };

//                            result.Add(row);
//                        }
//                    }
//                }
//            }

//            // final grouping
//            var final = result
//                .GroupBy(x => x.IdWall)
//                .Select(g =>
//                {
//                    var first = g.First();

//                    return new WallDataResponse
//                    {
//                        IdWall = first.IdWall,
//                        MemberDetailsId = first.MemberId,
//                        MemberName = first.MemberName,
//                        PostMessage = first.PostMessage,
//                        WallDate = first.WallDate,
//                        IsActive = 1,

//                        ImageUrl = string.IsNullOrWhiteSpace(first.ImageUrls)
//                            ? new List<string>()
//                            : first.ImageUrls.Split(',').ToList(),

//                        LikeCount = new LikeCountDto { Count = first.LikeCount },
//                        CommentCount = first.CommentCount,

//                        Comment = g
//                            .Where(c => c.CommentId != null)
//                            .Select(c => new CommentDto
//                            {
//                                IdWallPostComments = c.CommentId.Value,
//                                CommentMemberId = c.CommentMemberId.Value,
//                                CommentMemberName = c.CommentMemberName,
//                                CommentString = c.CommentText,
//                                CommentDate = c.CommentDate.Value
//                            }).ToList()
//                    };
//                })
//                .ToList();

//            return final;
//        }


//        // ---------------------------------------------------------
//        // SOFT DELETE COMMENT
//        // ---------------------------------------------------------
//        public async Task<bool> SoftDeleteCommentAsync(int commentId)
//        {
//            string sql = @"
//                UPDATE wall_post_comments 
//                SET active = 0
//                WHERE id_wall_post_comments = @id;
//                SELECT ROW_COUNT();";

//            var parameters = new[]
//            {
//                new MySqlParameter("@id", commentId)
//            };

//            int rows = await _context.Database.ExecuteScalarAsync(
//                sql,
//                r => r.GetInt32(0),
//                parameters
//            );

//            return rows > 0;
//        }

//        // ---------------------------------------------------------
//        // SOFT DELETE WALL (also delete comments + likes)
//        // ---------------------------------------------------------
//        public async Task<bool> SoftDeleteWallAsync(int wallId)
//        {
//            string sql = @"
//                UPDATE wall SET active = 0 WHERE id_wall = @wallId;
//                UPDATE wall_post_comments SET active = 0 WHERE wall_id = @wallId;
//                UPDATE wall_post_like SET active = 0 WHERE wall_id = @wallId;
//                SELECT 1;";

//            var parameters = new[]
//            {
//                new MySqlParameter("@wallId", wallId)
//            };

//            int result = await _context.Database.ExecuteScalarAsync(
//                sql,
//                r => r.GetInt32(0),
//                parameters
//            );

//            return result == 1;
//        }
//    }
//}










using Microsoft.EntityFrameworkCore;
using VHPProjectCommonUtility.Logger;
using VHPProjectDAL.DataModel;
using VHPProjectDTOModel.WallDTO.request;
using VHPProjectDTOModel.WallDTO.response;

namespace VHPProjectDAL.WallRepo
{
    public class WallRepository : IWallRepository
    {
        private readonly MasterProjContext _context;
        private readonly ILoggerManager _logger;

        public WallRepository(MasterProjContext context, ILoggerManager logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> AddCommentAsync(WallPostComments comment)
        {
            try
            {
                _logger.LogInfo("Adding comment...");
                comment.CreatedDate = DateTime.UtcNow;
                comment.Active = 1;

                await _context.WallPostComments.AddAsync(comment);
                await _context.SaveChangesAsync();

                _logger.LogInfo($"Comment added successfully: ID = {comment.IdwallPostComments}");
                return comment.IdwallPostComments;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error adding comment", ex);
                throw;
            }
        }

        public async Task AddOrUpdateLikeAsync(WallPostLike like)
        {
            try
            {
                var existing = await _context.WallPostLike
                    .FirstOrDefaultAsync(x => x.MemberDetailsId == like.MemberDetailsId && x.WallId == like.WallId);

                if (existing == null)
                {
                    _logger.LogInfo("Inserting new like entry...");
                    like.CreatedDate = DateTime.UtcNow;
                    like.Active = 1;

                    await _context.WallPostLike.AddAsync(like);
                }
                else
                {
                    _logger.LogInfo("Updating like entry...");
                    existing.Like = like.Like;
                    existing.Active = like.Active;
                    existing.CreatedDate = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
                _logger.LogInfo("Like updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error updating like", ex);
                throw;
            }
        }

        public async Task<int> AddWallAsync(Wall wall)
        {
            try
            {
                _logger.LogInfo("Adding new wall post...");
                wall.CreatedDate = DateTime.UtcNow;
                wall.Active = 1;

                await _context.Wall.AddAsync(wall);
                await _context.SaveChangesAsync();

                _logger.LogInfo($"Wall added successfully: ID = {wall.IdWall}");
                return wall.IdWall;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error adding wall", ex);
                throw;
            }
        }

        public async Task AddWallImagesAsync(IEnumerable<WallPostImages> images)
        {
            try
            {
                _logger.LogInfo("Adding wall images...");

                foreach (var img in images)
                {
                    img.CreatedDate = DateTime.UtcNow;
                    img.Active = 1;
                }

                await _context.WallPostImages.AddRangeAsync(images);
                await _context.SaveChangesAsync();

                _logger.LogInfo("Wall images added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error adding wall images", ex);
                throw;
            }
        }

        public async Task<WallPostLike?> GetLikeByMemberAndWallAsync(int memberId, int wallId)
        {
            try
            {
                return await _context.WallPostLike
                    .FirstOrDefaultAsync(x => x.MemberDetailsId == memberId && x.WallId == wallId && x.Active == 1);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error fetching like", ex);
                throw;
            }
        }

        public async Task<int> GetTotalWallCountAsync()
        {
            try
            {
                return await _context.Wall.CountAsync(w => w.Active == 1 || w.Active == 0);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error counting walls", ex);
                throw;
            }
        }

        public async Task<List<WallDataResponse>> GetWallDataAsync(GetWallDataRequest req, int skip, int take)
        {
            try
            {
                _logger.LogInfo($"Fetching wall data - Skip:{skip}, Take:{take}");

                var query = _context.Wall
                    .Where(w => w.Active == 1 || w.Active == 2)
                    .OrderByDescending(w => w.CreatedDate)
                    .Skip(skip).Take(take);

                var result = new List<WallDataResponse>();
                var walls = await query.ToListAsync();

                foreach (var w in walls)
                {
                    var member = await _context.MemberDetails.FirstOrDefaultAsync(m => m.IdMember == w.MemberDetailsId);

                    var images = await _context.WallPostImages
                        .Where(i => i.WallId == w.IdWall && i.Active == 1)
                        .Select(i => i.ImageUrl)
                        .ToListAsync();

                    var comments = await _context.WallPostComments
                        .Where(c => c.WallId == w.IdWall && c.Active == 1)
                        .OrderByDescending(c => c.InsertDateTime)
                        .ToListAsync();

                    var commentDtos = new List<CommentDto>();
                    foreach (var c in comments)
                    {
                        var commentMember = await _context.MemberDetails.FirstOrDefaultAsync(m => m.IdMember == c.MemberDetailsId);

                        commentDtos.Add(new CommentDto
                        {
                            IdWallPostComments = c.IdwallPostComments,
                            CommentString = c.CommentText,
                            CommentMemberId = (int)c.MemberDetailsId,
                            CommentMemberName = commentMember != null ? $"{commentMember.FirstName} {commentMember.LastName}" : null,
                            CommentDate = (DateTime)c.InsertDateTime
                        });
                    }

                    var likeCount = await _context.WallPostLike.CountAsync(l => l.WallId == w.IdWall && l.Active == 1 && l.Like == 1);

                    result.Add(new WallDataResponse
                    {
                        IdWall = w.IdWall,
                        MemberDetailsId = (int)w.MemberDetailsId,
                        MemberName = member != null ? $"{member.FirstName} {member.LastName}" : null,
                        PostMessage = w.PostMessages,
                        IsActive = (int)w.Active,
                        WallDate = (DateTime)w.CreatedDate,
                        ImageUrl = images,
                        Comment = commentDtos,
                        LikeCount = new LikeCountDto { Count = likeCount },
                        CommentCount = comments.Count
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error fetching wall data", ex);
                throw;
            }
        }

        public async Task<bool> SoftDeleteCommentAsync(int commentId)
        {
            try
            {
                var c = await _context.WallPostComments.FirstOrDefaultAsync(x => x.IdwallPostComments == commentId);

                if (c == null)
                {
                    _logger.LogWarn($"Comment not found: ID = {commentId}");
                    return false;
                }

                c.Active = 0;
                await _context.SaveChangesAsync();

                _logger.LogInfo($"Comment soft-deleted: ID = {commentId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error deleting comment", ex);
                throw;
            }
        }

        public async Task<bool> SoftDeleteWallAsync(int wallId)
        {
            try
            {
                var wall = await _context.Wall.FirstOrDefaultAsync(w => w.IdWall == wallId);

                if (wall == null)
                {
                    _logger.LogWarn($"Wall not found: ID = {wallId}");
                    return false;
                }

                wall.Active = 0;

                var comments = _context.WallPostComments.Where(c => c.WallId == wallId);
                foreach (var c in comments) c.Active = 0;

                var likes = _context.WallPostLike.Where(l => l.WallId == wallId);
                foreach (var l in likes) l.Active = 0;

                await _context.SaveChangesAsync();
                _logger.LogInfo($"Wall soft-deleted: ID = {wallId}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error deleting wall", ex);
                throw;
            }
        }
    }
}
