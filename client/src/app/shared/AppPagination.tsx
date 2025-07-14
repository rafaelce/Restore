import { Box, Pagination, Typography } from "@mui/material";
import type { Pagination as PaginationType } from "../../app/models/pagination";

type Props = {
  metadata: PaginationType;
  onPageChange: (page: number) => void;
};

export default function AppPagination({ metadata, onPageChange }: Props) {
    const { currentPage, totalPages, pageSize, totalCount } = metadata;

    const startItem = (currentPage - 1) * pageSize + 1;
    const endItem = Math.min(currentPage * pageSize, totalCount);
  
    return (
        <Box
            display="flex"
            justifyContent="space-between"
            alignItems="center"
            mt={3}
        >
        <Typography
            sx={{ color: (theme) => theme.palette.text.primary }}
        >
            Displaying {startItem}–{endItem} of {totalCount} items</Typography>
        <Pagination
            color="secondary"
            size="large"
            count={totalPages}
            page={currentPage}
            onChange={(_, page) => onPageChange(page)}
            sx={{
              '& .MuiPaginationItem-root': {
                color: '#1976d2',
                fontWeight: 600,
                borderRadius: '50%',
                border: '2px solid #1976d2',
                backgroundColor: '#fff',
                transition: 'all 0.2s',
                '&:hover': {
                  backgroundColor: '#e3f2fd',
                  boxShadow: '0 2px 8px rgba(25, 118, 210, 0.15)',
                },
              },
              '& .Mui-selected': {
                backgroundColor: '#1565c0 !important',
                color: '#fff !important',
                border: '2px solid #1565c0',
                boxShadow: '0 4px 12px rgba(21, 101, 192, 0.25)',
                fontWeight: 700,
              },
              '& .MuiPaginationItem-ellipsis': {
                border: 'none',
                background: 'none',
              }
            }}
        />
        </Box>
  );
}
