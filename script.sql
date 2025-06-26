select 
p.UniqueName as PlatformName,
w.*
from Wells w
left join Platforms p on p.id = w.PlatformId
where w.UpdatedAt = (select max(w2.UpdatedAt) from Wells w2 where w2.PlatformId = w.PlatformId)
order by p.id;
