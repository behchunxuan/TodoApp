import React, { useState, useEffect } from "react";
import { TodoItemDto } from "../types";
import { createTodo, updateTodo, deleteTodo, completeTodo, cancelTodo } from "../api/todoApi";
import Swal from "sweetalert2";

interface Props {
  open: boolean;
  item?: TodoItemDto | null;
  readonly?: boolean;
  onClose: () => void;
  onSaved: () => void;
}

const TodoModal: React.FC<Props> = ({ open, item, readonly = false, onClose, onSaved }) => {
  const [title, setTitle] = useState("");
  const [content, setContent] = useState("");
  const [priority, setPriority] = useState<"Low" | "Medium" | "High">("Low");
  const [status, setStatus] = useState<"Pending" | "Completed" | "Cancelled">("Pending");
  const [tag, setTag] = useState("");
  const [errors, setErrors] = useState<{ [key: string]: string[] }>({});

  const ValidationError = ({ field, errors }: { field: string, errors: Record<string, string[]> }) => {
    return errors[field] ? (
      <p className="text-sm text-red-600 mt-1">{errors[field][0]}</p>
    ) : null;
  };

  useEffect(() => {
    if (!open) return;

    if (item) {
      setTitle(item.title);
      setContent(item.content || "");
      setPriority(item.priority as "Low" | "Medium" | "High");
      setStatus(item.status as "Pending" | "Completed" | "Cancelled");
      setTag(item.tag || "");
    } else {
      setTitle("");
      setContent("");
      setPriority("Low");
      setStatus("Pending");
      setTag("");
    }

    setErrors({});
  }, [item, open]);

  const formatDateTime = (isoString: string) => {
    const date = new Date(isoString);
    return date.toLocaleString("en-MY", {
      year: "numeric",
      month: "short",
      day: "numeric",
      hour: "2-digit",
      minute: "2-digit",
    });
  };

  const handleSubmit = async () => {
    setErrors({}); // Clear previous errors

    const payload = {
      ...item,
      title,
      content,
      priority,
      status,
      tag,
    };

    try {
        const res = item ? await updateTodo(payload) : await createTodo(payload);

        // ✅ Show SweetAlert success message from backend
        await Swal.fire({
          icon: "success",
          title: "Success",
          text: res.data.message || "Operation successful.",
          confirmButtonText: "OK",
        });
        onSaved();
      } catch (err: any) {
        if (err.response?.status === 400 && err.response.data?.errors) {
          setErrors(err.response.data.errors);
        } else {
          Swal.fire({
            icon: "error",
            title: "Error",
            text: err.response?.data?.message || "Something went wrong.",
          });
        }
      }
  };

  const handleDelete = async () => {
    if (!item) return;

    const confirm = await Swal.fire({
      icon: "warning",
      title: "Are you sure?",
      text: "This will permanently delete the todo.",
      showCancelButton: true,
      confirmButtonText: "Yes, delete it!",
    });

    if (confirm.isConfirmed) {
      try {
        const res = await deleteTodo(item.todo_id);
        await Swal.fire({
          icon: "success",
          title: "Success",
          text: res.data.message || "Deleted successfully.",
          confirmButtonText: "OK",
        });
        onSaved();
      } catch (err: any) {
        Swal.fire({
          icon: "error",
          title: "Error",
          text: err.response?.data?.message || "Failed to delete todo.",
        });
      }
    }
  };

  const handleComplete = async () => {
    if (!item) return;
    try {
      const res = await completeTodo(item.todo_id);
      await Swal.fire({
        icon: "success",
        title: "Success",
        text: res.data.message || "Marked as completed.",
        confirmButtonText: "OK",
      });
      onSaved();
    } catch (err: any) {
      Swal.fire({
        icon: "error",
        title: "Error",
        text: err.response?.data?.message || "Failed to complete todo.",
      });
    }
  };

  const handleCancelTodo = async () => {
    if (!item) return;

    const confirm = await Swal.fire({
      icon: "warning",
      title: "Are you sure?",
      text: "This will mark the todo as cancelled.",
      showCancelButton: true,
      confirmButtonText: "Yes, cancel it!",
    });

    if (!confirm.isConfirmed) return;

    try {
      const res = await cancelTodo(item.todo_id);
      await Swal.fire({
        icon: "success",
        title: "Success",
        text: res.data.message || "Todo cancelled.",
        confirmButtonText: "OK",
      });
      onSaved();
    } catch (err: any) {
      Swal.fire({
        icon: "error",
        title: "Error",
        text: err.response?.data?.message || "Failed to cancel todo.",
      });
    }
  };


  if (!open) return null;

  return (
    <div className="fixed inset-0 bg-black bg-opacity-30 flex items-center justify-center z-50">
      <div className="bg-white p-6 md:p-8 rounded-2xl shadow-xl w-[95%] max-w-3xl">
        <h2 className="text-2xl font-semibold mb-6 text-center">
          {readonly ? "📋 Todo Details" : item ? "✏️ Edit Todo" : "🆕 New Todo"}
        </h2>

        <div className="space-y-4">
          <div>
            <label className="block text-sm font-medium mb-1">Title</label>
            <input
              type="text"
              className="w-full border px-4 py-2 rounded-md"
              value={title}
              onChange={(e) => setTitle(e.target.value)}
              placeholder="Enter title"
              disabled={readonly}
            />
            <ValidationError field="title" errors={errors} />
          </div>

          <div>
            <label className="block text-sm font-medium mb-1">Content</label>
            <textarea
              className="w-full border px-4 py-2 rounded-md min-h-[6rem] max-h-40 overflow-y-auto resize-y"
              value={content}
              onChange={(e) => setContent(e.target.value)}
              placeholder="Enter content"
              disabled={readonly}
            />
            <ValidationError field="content" errors={errors} />
          </div>

          <div className="flex gap-4">
            <div className="w-1/2">
              <label className="block text-sm font-medium mb-1">Priority</label>
              <select
                value={priority}
                onChange={(e) => setPriority(e.target.value as any)}
                className="w-full border px-3 py-2 rounded-md"
                disabled={readonly}
              >
                <option value="High">🔥 High</option>
                <option value="Medium">⏳ Medium</option>
                <option value="Low">🧘 Low</option>
              </select>
            </div>

            <div className="w-1/2">
              <label className="block text-sm font-medium mb-1">Tag</label>
              <input
                type="text"
                className="w-full border px-4 py-2 rounded-md"
                value={tag}
                onChange={(e) => setTag(e.target.value)}
                placeholder="e.g. work, study"
                disabled={readonly}
              />
              <ValidationError field="tag" errors={errors} />
            </div>
          </div>

        {item && (
          <div className="w-full">
            <label className="block text-sm font-medium mb-1">Status</label>
            <p className="w-full text-sm px-3 py-2 border rounded-md bg-gray-50 text-gray-600">
              {status === "Pending" && "🕓 Pending"}
              {status === "Completed" && "✅ Completed"}
              {status === "Cancelled" && "❌ Cancelled"}
            </p>
          </div>
        )}

          {readonly && item?.submitted_date && (
            <p className="text-sm text-gray-600">
              🕒 Submitted At: {formatDateTime(item.submitted_date)}
            </p>
          )}
          {readonly && item?.status === "Completed" && item.completed_date && (
            <p className="text-sm text-green-600">
              ✅ Completed At: {formatDateTime(item.completed_date)}
            </p>
          )}
          {readonly && item?.status === "Cancelled" && item.cancelled_date && (
            <p className="text-sm text-red-600">
              ❌ Cancelled At: {formatDateTime(item.cancelled_date)}
            </p>
          )}
        </div>

        <div className="flex flex-wrap justify-end gap-2 mt-6">
          <button
          onClick={onClose}
          className="px-4 py-2 border border-gray-300 text-gray-600 rounded-md hover:bg-gray-100"
        >
          Close
        </button>


          {item && !readonly && (
            <>
              <button
                onClick={handleDelete}
                className="px-4 py-2 border bg-red-600 text-white rounded-md hover:bg-red-700"
              >
                Delete ToDo
              </button>

              <button
                onClick={handleCancelTodo}
                className="px-4 py-2 border bg-yellow-600 text-white rounded-md hover:bg-yellow-700"
              >
                Cancel ToDo
              </button>

              <button
                onClick={handleSubmit}
                className="px-4 py-2 border bg-blue-600 text-white rounded-md hover:bg-blue-700"
              >
                Update ToDo
              </button>
              
              <button
                onClick={handleComplete}
                className="px-4 py-2 border bg-green-600 text-white rounded-md hover:bg-green-700"
              >
                Complete ToDo
              </button>
            </>
          )}

          {!item && !readonly && (
            <button
              onClick={handleSubmit}
              className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700"
            >
              Create
            </button>
          )}
        </div>
      </div>
    </div>
  );
};

export default TodoModal;
